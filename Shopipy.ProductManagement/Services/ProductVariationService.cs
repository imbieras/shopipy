using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.ProductManagement.Services;

public class ProductVariationService(IGenericRepository<ProductVariation> _variationRepository, IGenericRepository<Product> _productRepository) : IProductVariationService
{
    public async Task<ProductVariation> CreateVariationAsync(ProductVariation variation, int productId, int businessId)
    {
        var product = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
        if (product == null)
        {
            throw new ArgumentException("Product not found for the specified business.");
        }

        variation.ProductId = productId;

        return await _variationRepository.AddAsync(variation);
    }

    public async Task<IEnumerable<ProductVariation>> GetAllVariationsOfProductInBusinessAsync(int productId, int businessId, int? top = null, int? skip = null)
    {
        var product = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);

        if (product == null)
        {
            throw new ArgumentException("Product not found for the specified business.");
        }

        var variations = await _variationRepository.GetAllByConditionAsync(v => v.ProductId == productId);

        if (top.HasValue || skip.HasValue)
        {
            variations = variations
                .Skip(skip ?? 0)
                .Take(top ?? int.MaxValue);
        }

        return variations;
    }

    public async Task<int> GetVariationCountOfProductAsync(int productId, int businessId)
    {
        var product = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
        if (product == null)
        {
            throw new ArgumentException("Product not found for the specified business.");
        }

        var count = (await _variationRepository.GetAllByConditionAsync(v => v.ProductId == productId)).Count();
        return count;
    }

    public async Task<ProductVariation?> GetVariationByIdAsync(int variationId, int productId, int businessId)
    {
        var product = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
        if (product == null)
        {
            return null; 
        }

        return await _variationRepository.GetByConditionAsync(v => v.VariationId == variationId && v.ProductId == productId);
    }

    public async Task<ProductVariation> UpdateVariationAsync(ProductVariation variation)
    {
        return await _variationRepository.UpdateAsync(variation);
    }

    public async Task<bool> DeleteVariationAsync(int variationId, int productId, int businessId)
    {
        var existingVariation = await GetVariationByIdAsync(variationId, productId, businessId);
        if (existingVariation == null)
        {
            return false;
        }

        return await _variationRepository.DeleteAsync(existingVariation.VariationId);
    }
}
