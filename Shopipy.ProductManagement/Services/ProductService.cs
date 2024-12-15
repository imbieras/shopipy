using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.ProductManagement.Services;

public class ProductService(IGenericRepository<Product> _productRepository) : IProductService
{
    public async Task<Product> CreateProductAsync(Product product, int businessId)
    {
        product.BusinessId = businessId;

        var createdProduct = await _productRepository.AddAsync(product);
        return createdProduct;
    }

    public async Task<IEnumerable<Product>> GetAllProductsAsync(int businessId, int? categoryId = null, int? top = null, int? skip = 0)
    {
        if (skip.HasValue || top.HasValue)
        {
            return await _productRepository.GetAllByConditionWithPaginationAsync(
                p => p.BusinessId == businessId && (!categoryId.HasValue || p.CategoryId == categoryId),
                skip ?? 0,
                top ?? int.MaxValue
            );
        }

        return await _productRepository.GetAllByConditionAsync(
            p => p.BusinessId == businessId && (!categoryId.HasValue || p.CategoryId == categoryId)
        );
    }

    public async Task<int> GetProductCountAsync(int businessId, int? categoryId = null)
    {
        return await _productRepository.GetCountByConditionAsync(
            p => p.BusinessId == businessId && (!categoryId.HasValue || p.CategoryId == categoryId)
        );
    }

    public async Task<Product?> GetProductByIdAsync(int productId, int businessId)
    {
        var product = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
        return product;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        product.UpdatedAt = DateTime.UtcNow;
        return await _productRepository.UpdateAsync(product);
    }

    public async Task<bool> DeleteProductAsync(int productId, int businessId)
    {
        var product = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
        if (product == null) return false;

        return await _productRepository.DeleteAsync(product.ProductId);
    }
}
