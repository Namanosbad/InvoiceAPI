using Invoice.API.Domain.Enums;

namespace Invoice.API.Internal.Models.Invoices
{
    public class UpdateInvoiceStatusRequest
    {
        public InvoiceStatus Status { get; set; }
    }
}
