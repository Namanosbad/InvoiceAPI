namespace Invoice.API.Domain.Entities
{
    public class Client : Entity
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? CompanyName { get; set; }
        public string? TaxId { get; set; }
        public string? Address { get; set; }
        public virtual ICollection<Invoice> Invoices { get; set; } = new List<Invoice>();
    }
}
