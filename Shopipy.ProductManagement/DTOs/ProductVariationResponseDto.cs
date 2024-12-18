using Shopipy.Persistence.Models;

namespace Shopipy.ProductManagement.DTOs;

public class ProductVariationResponseDto
{
    public int VariationId { get; init; }

    public int ProductId { get; init; }

    public string Name { get; init; } = string.Empty;

    public decimal PriceModifier { get; init; }

    public ProductState ProductState { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}