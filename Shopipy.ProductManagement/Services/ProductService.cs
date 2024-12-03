using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.ProductManagement.Services
{
    public class ProductService : IProductService
    {
        private readonly IGenericRepository<Product> _productRepository;
        private readonly IGenericRepository<ProductVariation> _variationRepository;

        public ProductService(IGenericRepository<Product> productRepository, IGenericRepository<ProductVariation> variationRepository)
        {
            _productRepository = productRepository;
            _variationRepository = variationRepository;
        }

        public async Task<Product> CreateProductAsync(Product product, int businessId)
        {
            product.BusinessId = businessId;
            product.CreatedAt = DateTime.UtcNow;
            product.UpdatedAt = DateTime.UtcNow;

            var createdProduct = await _productRepository.AddAsync(product);
            return createdProduct;
        }

        public async Task<Product?> GetProductByIdAsync(int productId, int businessId)
        {
            var product = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
            return product;
        }

        public async Task<Product?> UpdateProductAsync(int productId, Product product, int businessId)
        {
            var existingProduct = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
            if (existingProduct == null) return null;

            existingProduct.Name = product.Name;
            existingProduct.Description = product.Description;
            existingProduct.BasePrice = product.BasePrice;
            existingProduct.ProductState = product.ProductState;
            existingProduct.CategoryId = product.CategoryId;
            existingProduct.UpdatedAt = DateTime.UtcNow;

            var updatedProduct = await _productRepository.UpdateAsync(existingProduct);
            return updatedProduct;
        }

        public async Task<bool> DeleteProductAsync(int productId, int businessId)
        {
            var product = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
            if (product == null) return false;

            return await _productRepository.DeleteAsync(product.ProductId);
        }
    }
}
