using Invoice.API.Domain.Enums;

namespace Invoice.API.Internal.Contracts.Responses;

/// <summary>
/// Retorno simplificado de uma fatura.
/// </summary>
public sealed class InvoiceResponse
{
    /// <summary>
    /// Identificador da fatura.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Identificador do cliente.
    /// </summary>
    public Guid ClientId { get; init; }

    /// <summary>
    /// Nome do cliente.
    /// </summary>
    public string? ClientName { get; init; }

    /// <summary>
    /// Data de emissão.
    /// </summary>
    public DateTime IssueDate { get; init; }

    /// <summary>
    /// Status atual da fatura.
    /// </summary>
    public InvoiceStatus Status { get; init; }

    /// <summary>
    /// Valor total.
    /// </summary>
    public decimal TotalAmount { get; init; }

    /// <summary>
    /// Itens da fatura.
    /// </summary>
    public List<InvoiceItemResponse> Items { get; init; } = [];
}
