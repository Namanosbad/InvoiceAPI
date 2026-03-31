namespace Invoice.API.Internal.Models.ServiceItems
{
    public class UpdateServiceItemRequest
    {
        public string Name { get; set; } = string.Empty;
        public decimal DefaultPrice { get; set; }
        public string UnitType { get; set; } = string.Empty;
    }
}
