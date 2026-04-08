using System.ComponentModel.DataAnnotations;

namespace Invoice.API.Application.Requests;

public sealed class CreateInvoiceItemRequest
{
    [Required]
    public Guid ServiceItemId { get; set; }

    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    public decimal? UnitPrice { get; set; }

    [MaxLength(300)]
    public string? Description { get; set; }
}
