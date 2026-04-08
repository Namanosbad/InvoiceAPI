using Invoice.API.Domain.Enums;

namespace Invoice.API.Internal.Contracts.Responses;

public sealed class InvoiceResponse
{
    public Guid Id { get; init; }
    public Guid ClientId { get; init; }
    public string? ClientName { get; init; }
    public DateTime IssueDate { get; init; }
    public InvoiceStatus Status { get; init; }
    public decimal TotalAmount { get; init; }
    public List<InvoiceItemResponse> Items { get; init; } = [];
}
