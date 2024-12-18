using Shopipy.Persistence.Models;

namespace Shopipy.ProductManagement.DTOs;

public class ProductVariationRequestDto
{
    public required string Name { get; set; }

    public required decimal PriceModifier { get; set; }

    public required ProductState ProductState { get; set; }
}