namespace Invoice.API.Internal.Contracts.Responses;

public sealed class ServiceItemResponse
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public decimal DefaultPrice { get; init; }
    public string? UnitType { get; init; }
}
