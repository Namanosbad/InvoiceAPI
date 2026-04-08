using System.ComponentModel.DataAnnotations;

namespace Invoice.API.Internal.Contracts.Requests;

/// <summary>
/// Dados necessários para criar uma fatura.
/// </summary>
public sealed class CreateInvoiceRequest
{
    /// <summary>
    /// Identificador do cliente.
    /// </summary>
    [Required]
    public Guid ClientId { get; set; }

    /// <summary>
    /// Data de emissão da fatura (opcional).
    /// </summary>
    public DateTime? IssueDate { get; set; }

    /// <summary>
    /// Itens da fatura.
    /// </summary>
    [Required]
    [MinLength(1)]
    public List<CreateInvoiceItemRequest> Items { get; set; } = [];
}
