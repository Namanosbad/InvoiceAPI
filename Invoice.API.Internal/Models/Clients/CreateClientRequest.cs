namespace Invoice.API.Internal.Models.Clients
{
    public class CreateClientRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? CompanyName { get; set; }
        public string? TaxId { get; set; }
        public string? Address { get; set; }
    }
}
