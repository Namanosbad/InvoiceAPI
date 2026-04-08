using System.ComponentModel.DataAnnotations;

namespace Invoice.API.Internal.Contracts.Requests;

public sealed class CreateInvoiceRequest
{
    [Required]
    public Guid ClientId { get; set; }

    public DateTime? IssueDate { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateInvoiceItemRequest> Items { get; set; } = [];
}
