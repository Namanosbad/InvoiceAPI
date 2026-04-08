using System.ComponentModel.DataAnnotations;

namespace Invoice.API.Internal.Contracts.Requests;

/// <summary>
/// Dados de um item na criação da fatura.
/// </summary>
public sealed class CreateInvoiceItemRequest
{
    /// <summary>
    /// Identificador do serviço/produto.
    /// </summary>
    [Required]
    public Guid ServiceItemId { get; set; }

    /// <summary>
    /// Quantidade do item.
    /// </summary>
    [Range(1, int.MaxValue)]
    public int Quantity { get; set; }

    /// <summary>
    /// Preço unitário (opcional). Se omitido, usa o padrão do serviço.
    /// </summary>
    public decimal? UnitPrice { get; set; }

    /// <summary>
    /// Descrição livre do item.
    /// </summary>
    [MaxLength(300)]
    public string? Description { get; set; }
}
