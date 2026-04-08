namespace Invoice.API.Internal.Contracts.Responses;

public sealed class InvoiceItemResponse
{
    public Guid ServiceItemId { get; init; }
    public string? Description { get; init; }
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal Total { get; init; }
}
