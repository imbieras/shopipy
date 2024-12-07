using Shopipy.Persistence.Models;

namespace Shopipy.ProductManagement.DTOs;

public class ProductVariationResponseDTO
{
    public int VariationId { get; set; }
    public int ProductId { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal PriceModifier { get; set; }
    public ProductState ProductState { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
