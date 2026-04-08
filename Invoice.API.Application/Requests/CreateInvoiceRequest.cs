using System.ComponentModel.DataAnnotations;

namespace Invoice.API.
public sealed class CreateInvoiceRequest
{
    [Required]
    public Guid ClientId { get; set; }

    public DateTime? IssueDate { get; set; }

    [Required]
    [MinLength(1)]
    public List<CreateInvoiceItemRequest> Items { get; set; } = [];
}
