namespace Invoice.API.Domain.Entities
{
    public class InvoiceItem : Entity
    {
        public string Description { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal UnitPrice { get; set; }
        public decimal Total {  get; set; }

        public Guid InvoiceId { get; set; }
        public virtual Invoice Invoice { get; set; } = null!;

        public Guid ServiceItemId { get; set; }
        public virtual ServiceItem ServiceItem { get; set; } = null!;
    }
}
