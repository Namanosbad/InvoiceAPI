using Invoice.API.Domain.Services;
using Invoice.API.Application.Requests;
using Invoice.API.Internal.Contracts.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace Invoice.API.Internal.Controllers;

/// <summary>
/// Endpoints para consulta e gerenciamento de faturas.
/// </summary>
[ApiController]
[Route("api/invoices")]
public sealed class InvoicesController(IInvoiceService invoiceService, IMapper mapper) : ControllerBase
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
        var invoices = await invoiceService.GetAllAsync(cancellationToken);
        return Ok(mapper.Map<List<InvoiceResponse>>(invoices));
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
        var invoice = await invoiceService.GetByIdAsync(id, cancellationToken);
        if (invoice is null)
        {
            return NotFound(new { message = "Invoice not found." });
        }

        return Ok(mapper.Map<InvoiceResponse>(invoice));
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
        var invoice = await invoiceService.GetByIdAsync(id, cancellationToken);
        if (invoice is null)
        {
            return NotFound(new { message = "Invoice not found." });
        }

        var pdfBytes = invoiceService.GeneratePdf(invoice);
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
        var createdInvoice = await invoiceService.CreateAsync(request, cancellationToken);
        var freshInvoice = await invoiceService.GetByIdAsync(createdInvoice.Id, cancellationToken);

        return CreatedAtAction(nameof(GetById), new { id = createdInvoice.Id }, mapper.Map<InvoiceResponse>(freshInvoice ?? createdInvoice));
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
        await invoiceService.UpdateStatusAsync(id, request.Status, cancellationToken);
        var updatedInvoice = await invoiceService.GetByIdAsync(id, cancellationToken);

        return Ok(mapper.Map<InvoiceResponse>(updatedInvoice!));
    }

}
