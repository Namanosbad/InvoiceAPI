using Invoice.API.Domain.Enums;

namespace Invoice.API.Internal.Models.Invoices
{
    public class UpdateInvoiceRequest
    {
        public DateTime IssueDate { get; set; }
        public InvoiceStatus Status { get; set; }
        public List<UpdateInvoiceItemRequest> Items { get; set; } = [];
    }

    public class UpdateInvoiceItemRequest
    {
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public Guid ServiceItemId { get; set; }
    }
}
