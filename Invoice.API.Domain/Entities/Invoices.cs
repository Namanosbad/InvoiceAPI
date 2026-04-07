using Invoice.API.Domain.Enums;

namespace Invoice.API.Domain.Entities
{
    public class Invoices : Entity
    {
        public Guid ClientId { get; set; }
        public DateTime IssueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public virtual Client Client { get; set; }

        public virtual ICollection<InvoiceItem> InvoiceItems { get; set; } = new List<InvoiceItem>();

        public void RecalculateTotalAmount()
        {
            TotalAmount = InvoiceItems.Sum(item => item.Total);
        }
    }
}
