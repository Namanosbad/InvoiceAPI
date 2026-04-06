namespace Invoice.API.Domain.Entities
{
    public class InvoiceItem : Entity
    {
        public string? Description { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total {  get; set; }

        public Guid InvoiceId { get; set; }
        public virtual Invoices Invoice { get; set; }

        public Guid ServiceItemId { get; set; }
        public virtual ServiceItem ServiceItem { get; set; }
    }
}