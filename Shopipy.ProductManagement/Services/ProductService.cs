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

    public async Task<IEnumerable<Product>> GetAllProductsOfBusinessAsync(int businessId, int? top = null, int? skip = null)
    {
        var products = await _productRepository.GetAllByConditionAsync(p => p.BusinessId == businessId);

        if (skip.HasValue)
        {
            products = products.Skip(skip.Value);
        }

        if (top.HasValue)
        {
            products = products.Take(top.Value);
        }

        return products;
    }

    public async Task<int> GetProductCountAsync(int businessId)
    {
        var products = await _productRepository.GetAllByConditionAsync(p => p.BusinessId == businessId);
        return products.Count();
    }

    public async Task<Product?> GetProductByIdAsync(int productId, int businessId)
    {
        var product = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
        return product;
    }

    public async Task<Product> UpdateProductAsync(Product product)
    {
        return await _productRepository.UpdateAsync(product);
    }

    public async Task<bool> DeleteProductAsync(int productId, int businessId)
    {
        var product = await _productRepository.GetByConditionAsync(p => p.ProductId == productId && p.BusinessId == businessId);
        if (product == null) return false;

        return await _productRepository.DeleteAsync(product.ProductId);
    }
}
