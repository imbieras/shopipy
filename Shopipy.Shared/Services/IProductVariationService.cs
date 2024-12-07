using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IProductVariationService
{
    Task<ProductVariation> CreateVariationAsync(ProductVariation variation, int productId, int businessId);
    Task<IEnumerable<ProductVariation>> GetAllVariationsOfProductInBusinessAsync(int productId, int businessId, int? top = null, int? skip = null);
    Task<int> GetVariationCountOfProductAsync(int productId, int businessId);
    Task<ProductVariation?> GetVariationByIdAsync(int variationId, int productId, int businessId);
    Task<ProductVariation> UpdateVariationAsync(ProductVariation variation);
    Task<bool> DeleteVariationAsync(int variationId, int productId, int businessId);
}
