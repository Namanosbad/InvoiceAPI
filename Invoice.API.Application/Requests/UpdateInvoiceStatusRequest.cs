using Invoice.API.Domain.Enums;
using System.ComponentModel.DataAnnotations;

namespace Invoice.API.Application.Requests;

public sealed class UpdateInvoiceStatusRequest
{
    [Required]
    public InvoiceStatus Status { get; set; }
}
