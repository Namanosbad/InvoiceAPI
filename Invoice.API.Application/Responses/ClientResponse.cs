namespace Invoice.API.Internal.Contracts.Responses;

public sealed class ClientResponse
{
    public Guid Id { get; init; }
    public string? Name { get; init; }
    public string? Email { get; init; }
    public string? CompanyName { get; init; }
    public string? TaxId { get; init; }
    public string? Address { get; init; }
}
