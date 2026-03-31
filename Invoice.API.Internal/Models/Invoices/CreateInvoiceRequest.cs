using Invoice.API.Domain.Enums;

namespace Invoice.API.Internal.Models.Invoices
{
    public class CreateInvoiceRequest
    {
        public Guid ClientId { get; set; }
        public DateTime IssueDate { get; set; }
        public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
        public List<CreateInvoiceItemRequest> Items { get; set; } = [];
    }

    public class CreateInvoiceItemRequest
    {
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid ServiceItemId { get; set; }
    }
}
