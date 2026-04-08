namespace Invoice.API.Internal.Contracts.Responses;

/// <summary>
/// Retorno simplificado de item de fatura.
/// </summary>
public sealed class InvoiceItemResponse
{
    /// <summary>
    /// Identificador do serviço/produto.
    /// </summary>
    public Guid ServiceItemId { get; init; }

    /// <summary>
    /// Descrição do item.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Quantidade do item.
    /// </summary>
    public int Quantity { get; init; }

    /// <summary>
    /// Preço unitário aplicado.
    /// </summary>
    public decimal UnitPrice { get; init; }

    /// <summary>
    /// Valor total do item.
    /// </summary>
    public decimal Total { get; init; }
}
