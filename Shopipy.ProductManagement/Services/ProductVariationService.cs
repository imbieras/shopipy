using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.ProductManagement.Services
{
    public class ProductVariationService : IProductVariationService
    {
        private readonly IGenericRepository<ProductVariation> _variationRepository;
        private readonly IGenericRepository<Product> _productRepository;

        public ProductVariationService(IGenericRepository<ProductVariation> variationRepository, IGenericRepository<Product> productRepository)
        {
            _variationRepository = variationRepository;
            _productRepository = productRepository;
        }

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

        public async Task<ProductVariation?> GetVariationByIdAsync(int variationId, int productId, int businessId)
        {
            var product = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
            if (product == null)
            {
                return null; 
            }

            return await _variationRepository.GetByConditionAsync(v => v.VariationId == variationId && v.ProductId == productId);
        }

        public async Task<ProductVariation?> UpdateVariationAsync(int variationId, ProductVariation variation, int productId, int businessId)
        {
            var existingVariation = await GetVariationByIdAsync(variationId, productId, businessId);
            if (existingVariation == null)
            {
                return null;
            }

            existingVariation.Name = variation.Name;
            existingVariation.PriceModifier = variation.PriceModifier;
            existingVariation.ProductState = variation.ProductState;
            existingVariation.UpdatedAt = variation.UpdatedAt;

            return await _variationRepository.UpdateAsync(existingVariation);
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
}
