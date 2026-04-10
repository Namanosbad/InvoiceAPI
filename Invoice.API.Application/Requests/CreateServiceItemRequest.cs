using System.ComponentModel.DataAnnotations;

namespace Invoice.API.Application.Requests;

public sealed class CreateServiceItemRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [Range(0, double.MaxValue)]
    public decimal DefaultPrice { get; set; }

    [MaxLength(50)]
    public string? UnitType { get; set; }
}
