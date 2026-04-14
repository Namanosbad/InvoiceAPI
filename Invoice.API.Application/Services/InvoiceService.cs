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
            return Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Margin(24);
                    page.Size(PageSizes.A4);
                    page.DefaultTextStyle(x => x.FontSize(11));

                    page.Header().Column(column =>
                    {
                        column.Item().Text("Invoice").FontSize(24).SemiBold();
                        column.Item().Text($"Invoice ID: {invoice.Id}");
                        column.Item().Text($"Issue Date: {invoice.IssueDate:yyyy-MM-dd HH:mm} UTC");
                        column.Item().Text($"Status: {invoice.Status}");
                    });

                    page.Content().PaddingVertical(16).Column(column =>
                    {
                        column.Spacing(10);

                        column.Item().Text("Client").FontSize(14).SemiBold();
                        column.Item().Text(invoice.Client?.Name ?? "N/A");
                        column.Item().Text(invoice.Client?.Email ?? "N/A");
                        column.Item().Text(invoice.Client?.CompanyName ?? "N/A");
                        column.Item().Text(invoice.Client?.Address ?? "N/A");

                        column.Item().PaddingTop(8).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(4);
                                columns.RelativeColumn(1);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Element(CellHeaderStyle).Text("Description");
                                header.Cell().Element(CellHeaderStyle).AlignRight().Text("Qty");
                                header.Cell().Element(CellHeaderStyle).AlignRight().Text("Unit Price");
                                header.Cell().Element(CellHeaderStyle).AlignRight().Text("Total");
                            });

                            foreach (var item in invoice.InvoiceItems)
                            {
                                table.Cell().Element(CellBodyStyle).Text(item.Description ?? "Item");
                                table.Cell().Element(CellBodyStyle).AlignRight().Text(item.Quantity.ToString());
                                table.Cell().Element(CellBodyStyle).AlignRight().Text($"{item.UnitPrice:0.00}");
                                table.Cell().Element(CellBodyStyle).AlignRight().Text($"{item.Total:0.00}");
                            }
                        });
                    });

                    page.Footer().AlignRight().Text($"Total: {invoice.TotalAmount:0.00}").FontSize(16).SemiBold();
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

        private static IContainer CellHeaderStyle(IContainer container)
        {
            return container
                .Background(Colors.Grey.Lighten2)
                .PaddingVertical(6)
                .PaddingHorizontal(6)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Darken1);
        }

        private static IContainer CellBodyStyle(IContainer container)
        {
            return container
                .PaddingVertical(6)
                .PaddingHorizontal(6)
                .BorderBottom(1)
                .BorderColor(Colors.Grey.Lighten2);
        }
    }
}
