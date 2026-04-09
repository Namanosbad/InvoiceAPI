using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Domain.Services;
using Invoice.API.Application.Requests;
using Invoice.API.Internal.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Invoice.API.Internal.Controllers;

/// <summary>
/// Endpoints para consulta e gerenciamento de faturas.
/// </summary>
[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController(IInvoiceService invoiceService, IInvoiceRepository invoiceRepository) : ControllerBase
{
    /// <summary>
    /// Lista todas as faturas.
    /// </summary>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Lista simples de faturas.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(List<InvoiceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var invoices = await invoiceRepository.GetAllAsync(cancellationToken);
        return Ok(invoices.Select(ToResponse));
    }

    /// <summary>
    /// Busca uma fatura pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da fatura.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Fatura encontrada.</returns>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetByIdAsync(id, cancellationToken);
        if (invoice is null)
        {
            return NotFound(new { message = "Invoice not found." });
        }

        return Ok(ToResponse(invoice));
    }

    /// <summary>
    /// Gera o PDF de uma fatura específica.
    /// </summary>
    /// <param name="id">Identificador da fatura.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Arquivo PDF da fatura.</returns>
    [HttpGet("{id:guid}/pdf")]
    [Produces("application/pdf")]
    [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DownloadPdf(Guid id, CancellationToken cancellationToken)
    {
        var invoice = await invoiceRepository.GetByIdAsync(id, cancellationToken);
        if (invoice is null)
        {
            return NotFound(new { message = "Invoice not found." });
        }

        var pdfBytes = BuildInvoicePdf(invoice);
        var fileName = $"invoice-{invoice.Id:N}.pdf";
        return File(pdfBytes, "application/pdf", fileName);
    }

    /// <summary>
    /// Cria uma nova fatura.
    /// </summary>
    /// <param name="request">Dados de criação da fatura.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Fatura criada.</returns>
    [HttpPost]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateInvoiceRequest request, CancellationToken cancellationToken)
    {
        try
        {
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

            var createdInvoice = await invoiceService.CreateAsync(invoice, cancellationToken);
            var freshInvoice = await invoiceRepository.GetByIdAsync(createdInvoice.Id, cancellationToken);

            return CreatedAtAction(nameof(GetById), new { id = createdInvoice.Id }, ToResponse(freshInvoice ?? createdInvoice));
        }
        catch (Exception ex) when (ex is InvalidOperationException or KeyNotFoundException)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    /// <summary>
    /// Atualiza o status de uma fatura.
    /// </summary>
    /// <param name="id">Identificador da fatura.</param>
    /// <param name="request">Novo status.</param>
    /// <param name="cancellationToken">Token de cancelamento da requisição.</param>
    /// <returns>Fatura atualizada.</returns>
    [HttpPatch("{id:guid}/status")]
    [ProducesResponseType(typeof(InvoiceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdateInvoiceStatusRequest request, CancellationToken cancellationToken)
    {
        try
        {
            await invoiceService.UpdateStatusAsync(id, request.Status, cancellationToken);
            var updatedInvoice = await invoiceRepository.GetByIdAsync(id, cancellationToken);

            return Ok(ToResponse(updatedInvoice!));
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new { message = ex.Message });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new { message = ex.Message });
        }
    }

    private static InvoiceResponse ToResponse(Invoices invoice)
    {
        return new InvoiceResponse
        {
            Id = invoice.Id,
            ClientId = invoice.ClientId,
            ClientName = invoice.Client?.Name,
            IssueDate = invoice.IssueDate,
            Status = invoice.Status,
            TotalAmount = invoice.TotalAmount,
            Items = invoice.InvoiceItems.Select(item => new InvoiceItemResponse
            {
                ServiceItemId = item.ServiceItemId,
                Description = item.Description,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                Total = item.Total
            }).ToList()
        };
    }

    private static byte[] BuildInvoicePdf(Invoices invoice)
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
