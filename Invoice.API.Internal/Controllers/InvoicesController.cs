using Invoice.API.Domain.Entities;
using Invoice.API.Domain.Repositories;
using Invoice.API.Domain.Services;
using Invoice.API.Internal.Contracts.Requests;
using Invoice.API.Internal.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Internal.Controllers;

[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController(IInvoiceService invoiceService, IInvoiceRepository invoiceRepository) : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(typeof(List<InvoiceResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var invoices = await invoiceRepository.GetAllAsync(cancellationToken);
        return Ok(invoices.Select(ToResponse));
    }

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
}
