namespace Invoice.API.Domain.Entities
{
    public class ServiceItem : Entity
    {
        public string? Name { get; set; }
        public decimal DefaultPrice { get; set; }
        public string? UnitType { get; set; }
    }
}