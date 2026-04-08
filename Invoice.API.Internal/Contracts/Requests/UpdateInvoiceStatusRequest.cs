using Invoice.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Invoice.API.Internal.Contracts.Requests;

public sealed class UpdateInvoiceStatusRequest
{
    [Required]
    public InvoiceStatus Status { get; set; }
}
