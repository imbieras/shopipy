using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.ProductManagement.Services;

public class ProductVariationService(IGenericRepository<ProductVariation> variationRepository, IGenericRepository<Product> productRepository) : IProductVariationService
{
    public async Task<ProductVariation> CreateVariationAsync(ProductVariation variation, int productId, int businessId)
    {
        var product = await productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
        if (product == null)
        {
            throw new ArgumentException("Product not found for the specified business.");
        }

        variation.ProductId = productId;

        return await variationRepository.AddAsync(variation);
    }

    public async Task<IEnumerable<ProductVariation>> GetAllVariationsOfProductInBusinessAsync(int productId, int businessId, int? top = null, int? skip = 0)
    {
        var product = await productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);

        if (product == null)
        {
            throw new ArgumentException("Product not found for the specified business.");
        }

        return await variationRepository.GetAllByConditionWithPaginationAsync(
        v => v.ProductId == productId,
        skip ?? 0,
        top ?? int.MaxValue
        );
    }

    public async Task<int> GetVariationCountOfProductAsync(int productId, int businessId)
    {
        var product = await productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);

        if (product == null)
        {
            throw new ArgumentException("Product not found for the specified business.");
        }

        return await variationRepository.GetCountByConditionAsync(v => v.ProductId == productId);
    }

    public async Task<ProductVariation?> GetVariationByIdInBusinessAsync(int variationId, int productId, int businessId)
    {
        var product = await productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
        if (product == null)
        {
            return null;
        }

        return await variationRepository.GetByConditionAsync(v => v.VariationId == variationId && v.ProductId == productId);
    }

    public async Task<ProductVariation> UpdateVariationAsync(ProductVariation variation)
    {
        variation.UpdatedAt = DateTime.UtcNow;
        return await variationRepository.UpdateAsync(variation);
    }

    public async Task<bool> DeleteVariationAsync(int variationId, int productId, int businessId)
    {
        var existingVariation = await GetVariationByIdInBusinessAsync(variationId, productId, businessId);
        if (existingVariation == null)
        {
            return false;
        }

        return await variationRepository.DeleteAsync(existingVariation.VariationId);
    }
}