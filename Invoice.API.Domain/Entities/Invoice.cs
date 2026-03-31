using Invoice.API.Domain.Enums;

namespace Invoice.API.Domain.Entities
{
    public class Invoice : Entity
    {
        public Guid ClientId { get; set; }
        public DateTime IssueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public virtual Client Client { get; set; } = null!;

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();
    }
}
