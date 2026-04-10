using System.ComponentModel.DataAnnotations;

namespace Invoice.API.Application.Requests;

public sealed class CreateClientRequest
{
    [Required]
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [EmailAddress]
    [MaxLength(200)]
    public string? Email { get; set; }

    [MaxLength(200)]
    public string? CompanyName { get; set; }

    [MaxLength(50)]
    public string? TaxId { get; set; }

    [MaxLength(500)]
    public string? Address { get; set; }
}
