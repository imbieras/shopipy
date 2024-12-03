using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IProductVariationService
{
    Task<ProductVariation> CreateVariationAsync(ProductVariation variation, int productId, int businessId);
    Task<ProductVariation?> GetVariationByIdAsync(int variationId, int productId, int businessId);
    Task<ProductVariation?> UpdateVariationAsync(int variationId, ProductVariation variation, int productId, int businessId);
    Task<bool> DeleteVariationAsync(int variationId, int productId, int businessId);
}
