using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IProductService
{
    Task<Product> CreateProductAsync(Product product, int businessId);
    Task<Product?> GetProductByIdAsync(int productId, int businessId);
    Task<Product?> UpdateProductAsync(int productId, Product product, int businessId);
    Task<bool> DeleteProductAsync(int productId, int businessId);
}
