using System.Globalization;
using Invoice.API.Domain.Repositories;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Invoice.API.Domain.Services;

public sealed class InvoicePdfService(IInvoiceRepository invoiceRepository) : IInvoicePdfService
{
    public async Task<byte[]> GenerateAsync(Guid invoiceId, CancellationToken cancellationToken = default)
    {
        var invoice = await invoiceRepository.GetByIdAsync(invoiceId, cancellationToken)
            ?? throw new KeyNotFoundException("Invoice not found.");

        QuestPDF.Settings.License = LicenseType.Community;

        var culture = CultureInfo.GetCultureInfo("pt-BR");

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(11));

                page.Header()
                    .Column(column =>
                    {
                        column.Item().Text("Fatura").Bold().FontSize(24);
                        column.Item().Text($"ID: {invoice.Id}");
                        column.Item().Text($"Data de emissão: {invoice.IssueDate:dd/MM/yyyy}");
                        column.Item().Text($"Status: {invoice.Status}");
                    });

                page.Content()
                    .PaddingVertical(20)
                    .Column(column =>
                    {
                        column.Spacing(8);
                        column.Item().Text($"Cliente: {invoice.Client?.Name ?? "N/A"}").Bold();
                        column.Item().Text($"E-mail: {invoice.Client?.Email ?? "N/A"}");
                        column.Item().Text($"Empresa: {invoice.Client?.CompanyName ?? "N/A"}");
                        column.Item().Text($"Endereço: {invoice.Client?.Address ?? "N/A"}");

                        column.Item().PaddingTop(10).Table(table =>
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
                                header.Cell().Element(CellStyle).Text("Descrição").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Qtd").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Unitário").Bold();
                                header.Cell().Element(CellStyle).AlignRight().Text("Total").Bold();
                            });

                            foreach (var item in invoice.InvoiceItems)
                            {
                                table.Cell().Element(CellStyle).Text(item.Description ?? "Sem descrição");
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Quantity.ToString());
                                table.Cell().Element(CellStyle).AlignRight().Text(item.UnitPrice.ToString("C", culture));
                                table.Cell().Element(CellStyle).AlignRight().Text(item.Total.ToString("C", culture));
                            }
                        });
                    });

                page.Footer()
                    .AlignRight()
                    .Text($"Total da fatura: {invoice.TotalAmount.ToString("C", culture)}")
                    .Bold()
                    .FontSize(13);
            });
        });

        return document.GeneratePdf();
    }

    private static IContainer CellStyle(IContainer container)
    {
        return container
            .BorderBottom(1)
            .BorderColor(Colors.Grey.Lighten2)
            .PaddingVertical(6)
            .PaddingHorizontal(4);
    }
}
