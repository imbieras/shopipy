using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IProductService
{
    Task<Product> CreateProductAsync(Product product, int businessId);
    Task<IEnumerable<Product>> GetAllProductsOfBusinessAsync(int businessId, int? top = null, int? skip = null);
    Task<int> GetProductCountAsync(int businessId);
    Task<Product?> GetProductByIdAsync(int productId, int businessId);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int productId, int businessId);
}
