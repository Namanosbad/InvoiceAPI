using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Enums;
using Invoice.API.Domain.Repositories;
using Invoice.API.Application.Requests;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Invoice.API.Domain.Services
{
    public sealed class InvoiceService(
        IInvoiceRepository invoiceRepository,
        IClientRepository clientRepository,
        IServiceItemRepository serviceItemRepository) : IInvoiceService
    {
        public Task<IReadOnlyList<Invoices>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return invoiceRepository.GetAllAsync(cancellationToken);
        }

        public Task<Invoices?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return invoiceRepository.GetByIdAsync(id, cancellationToken);
        }

        public async Task<Invoices> CreateAsync(CreateInvoiceRequest request, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(request);

            var invoice = new Invoices
            {
                ClientId = request.ClientId,
                IssueDate = request.IssueDate ?? DateTime.UtcNow,
                InvoiceItems = request.Items.Select(item => new InvoiceItem
                {
                    ServiceItemId = item.ServiceItemId,
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice ?? 0,
                    Description = item.Description
                }).ToList()
            };

            if (invoice.InvoiceItems is null || invoice.InvoiceItems.Count == 0)
            {
                throw new InvalidOperationException("Invoice must have at least one item.");
            }

            var client = await clientRepository.GetByIdAsync(invoice.ClientId, cancellationToken);
            if (client is null)
            {
                throw new KeyNotFoundException("Client not found.");
            }

            invoice.IssueDate = invoice.IssueDate == default ? DateTime.UtcNow : invoice.IssueDate;
            invoice.ClientId = client.Id;

            foreach (var item in invoice.InvoiceItems)
            {
                await NormalizeItemAsync(invoice.Id, item, cancellationToken);
            }

            invoice.RecalculateTotalAmount();

            await invoiceRepository.AddAsync(invoice, cancellationToken);
            return invoice;
        }

        public async Task<Invoices> UpdateStatusAsync(Guid invoiceId, InvoiceStatus newStatus, CancellationToken cancellationToken = default)
        {
            var invoice = await invoiceRepository.GetByIdAsync(invoiceId, cancellationToken)
                ?? throw new KeyNotFoundException("Invoice not found.");

            if (!IsStatusTransitionAllowed(invoice.Status, newStatus))
            {
                throw new InvalidOperationException($"Invalid status transition: {invoice.Status} -> {newStatus}.");
            }

            invoice.Status = newStatus;
            await invoiceRepository.UpdateAsync(invoice, cancellationToken);

            return invoice;
        }

        public byte[] GeneratePdf(Invoices invoice)
        {
            var accentColor = "#1a56db";

            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(45);
                    page.Size(PageSizes.A4);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(10).FontColor(Colors.Grey.Darken4).FontFamily(Fonts.Arial));

                    // --- CABEÇALHO (Logo e ID da Invoice) ---
                    page.Header().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text("MTECH LTDA.").FontSize(14).ExtraBold().FontColor(accentColor);
                            col.Item().Text("São Paulo, SP").FontSize(9).FontColor(Colors.Grey.Medium);
                        });

                        row.RelativeItem().AlignRight().Column(col =>
                        {
                            col.Item().Text("INVOICE").FontSize(22).ExtraLight();
                            col.Item().Text(x =>
                            {
                                x.Span("No. ").SemiBold();
                                x.Span($"{invoice.Id}");
                            });
                        });
                    });

                    page.Content().PaddingVertical(40).Column(column =>
                    {
                        // --- BARRA DE INFORMAÇÕES (Dados da API) ---
                        column.Item().BorderBottom(1).BorderColor(Colors.Grey.Lighten3).PaddingBottom(10).Row(row =>
                        {
                            // Dados do Cliente
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("BILL TO").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                                c.Item().PaddingTop(2).Text(invoice.Client?.Name ?? "N/A").SemiBold();
                                c.Item().Text(invoice.Client?.Email ?? "N/A").FontSize(9);
                                c.Item().Text(invoice.Client?.CompanyName ?? "").FontSize(9);
                            });

                            // Datas e Status
                            row.RelativeItem().Column(c =>
                            {
                                c.Item().Text("ISSUE DATE").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                                c.Item().PaddingTop(2).Text(invoice.IssueDate.ToString("yyyy-MM-dd HH:mm") + " UTC");

                                c.Item().PaddingTop(5).Text("STATUS").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                                c.Item().Text(invoice.Status.ToString()).FontSize(9);
                            });

                            // Valor de Destaque
                            row.RelativeItem().AlignRight().Column(c =>
                            {
                                c.Item().Text("TOTAL DUE").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                                c.Item().PaddingTop(2).Text($"$ {invoice.TotalAmount:0.00}").FontSize(14).SemiBold().FontColor(accentColor);
                            });
                        });

                        // --- TABELA DE ITENS (Loop da API) ---
                        column.Item().PaddingTop(30).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(6); // Descrição
                                columns.RelativeColumn(2); // Qty
                                columns.RelativeColumn(2); // Unit Price
                                columns.RelativeColumn(2); // Total
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(HeaderStyle).Text("Service Description");
                                header.Cell().Element(HeaderStyle).AlignCenter().Text("Qty");
                                header.Cell().Element(HeaderStyle).AlignRight().Text("Rate");
                                header.Cell().Element(HeaderStyle).AlignRight().Text("Amount");

                                static IContainer HeaderStyle(IContainer c) =>
                                    c.BorderBottom(1.5f).BorderColor(Colors.Black).PaddingVertical(5).DefaultTextStyle(x => x.SemiBold().FontSize(9));
                            });

                            // Percorre os itens vindos da sua API
                            foreach (var item in invoice.InvoiceItems)
                            {
                                table.Cell().Element(RowStyle).Column(c =>
                                {
                                    c.Item().Text(item.Description ?? "Item").SemiBold();
                                    // Aqui você pode adicionar uma sub-descrição se sua API tiver
                                });

                                table.Cell().Element(RowStyle).AlignCenter().Text(item.Quantity.ToString());
                                table.Cell().Element(RowStyle).AlignRight().Text($"$ {item.UnitPrice:0.00}");
                                table.Cell().Element(RowStyle).AlignRight().Text($"$ {item.Total:0.00}");
                            }

                            static IContainer RowStyle(IContainer c) =>
                                c.BorderBottom(1).BorderColor(Colors.Grey.Lighten4).PaddingVertical(10);
                        });

                        // --- TOTAIS FINAIS ---
                        column.Item().AlignRight().PaddingTop(20).Width(150).Column(c =>
                        {
                            c.Item().Row(r =>
                            {
                                r.RelativeItem().Text("Subtotal:");
                                r.RelativeItem().AlignRight().Text($"$ {invoice.TotalAmount:0.00}");
                            });

                            c.Item().PaddingTop(5).BorderTop(1).BorderColor(Colors.Black).Row(r =>
                            {
                                r.RelativeItem().Text("Total:").SemiBold().FontSize(12);
                                r.RelativeItem().AlignRight().Text($"$ {invoice.TotalAmount:0.00}").SemiBold().FontSize(12);
                            });
                        });

                        // --- NOTAS MENTAIS / RODAPÉ DO CONTEÚDO ---
                        column.Item().PaddingTop(50).Column(c =>
                        {
                            c.Item().Text("PAYMENT INSTRUCTIONS").FontSize(8).SemiBold().FontColor(Colors.Grey.Medium);
                            c.Item().PaddingTop(2).Text("Please pay within 15 days using the provided bank details.").FontSize(9);
                        });
                    });

                    // --- RODAPÉ DA PÁGINA ---
                    page.Footer().AlignRight().Text(x =>
                    {
                        x.Span("Page ").FontSize(9);
                        x.CurrentPageNumber().FontSize(9);
                    });
                });
            }).GeneratePdf();
        }

        private async Task NormalizeItemAsync(Guid invoiceId, InvoiceItem item, CancellationToken cancellationToken)
        {
            if (item.Quantity <= 0)
            {
                throw new InvalidOperationException("Invoice item quantity must be greater than zero.");
            }

            var serviceItem = await serviceItemRepository.GetByIdAsync(item.ServiceItemId, cancellationToken);
            if (serviceItem is null)
            {
                throw new KeyNotFoundException("Service item not found.");
            }

            if (item.UnitPrice < 0)
            {
                throw new InvalidOperationException("Invoice item unit price cannot be negative.");
            }
            
            var unitPrice = item.UnitPrice == 0 ? serviceItem.DefaultPrice : item.UnitPrice;

            item.InvoiceId = invoiceId;
            item.ServiceItemId = serviceItem.Id;
            item.Description = string.IsNullOrWhiteSpace(item.Description) ? serviceItem.Name : item.Description;
            item.UnitPrice = unitPrice;
            item.RecalculateTotal();
        }

        private static bool IsStatusTransitionAllowed(InvoiceStatus current, InvoiceStatus next)
        {
            if (current == next)
            {
                return true;
            }

            if (current == InvoiceStatus.Draft)
            {
                return next == InvoiceStatus.Pending || next == InvoiceStatus.Cancelled;
            }

            if (current == InvoiceStatus.Pending)
            {
                return next == InvoiceStatus.Paid || next == InvoiceStatus.Overdue || next == InvoiceStatus.Cancelled;
            }

            if (current == InvoiceStatus.Overdue)
            {
                return next == InvoiceStatus.Paid || next == InvoiceStatus.Cancelled;
            }

            return false;
        }
    }
}