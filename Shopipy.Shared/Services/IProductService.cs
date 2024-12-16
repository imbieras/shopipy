using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IProductService
{
    Task<Product> CreateProductAsync(Product product, int businessId);
    Task<IEnumerable<Product>> GetAllProductsAsync(int businessId, int? categoryId = null, int? top = null, int? skip = 0);
    Task<int> GetProductCountAsync(int businessId, int? categoryId = null);
    Task<Product?> GetProductByIdAsync(int productId, int businessId);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int productId, int businessId);
}
