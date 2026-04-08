using Invoice.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Invoice.API.Internal.Contracts.Requests;

/// <summary>
/// Dados para atualização de status da fatura.
/// </summary>
public sealed class UpdateInvoiceStatusRequest
{
    /// <summary>
    /// Novo status da fatura.
    /// </summary>
    [Required]
    public InvoiceStatus Status { get; set; }
}
