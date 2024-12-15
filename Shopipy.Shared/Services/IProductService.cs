using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IProductService
{
    Task<Product> CreateProductAsync(Product product, int businessId);
    Task<IEnumerable<Product>> GetAllProductsOfBusinessAsync(int businessId, int? top = null, int? skip = null);
    Task<IEnumerable<Product>> GetAllProductsOfCategoryBusinessAsync(int businessId, int categoryId, int? top = null, int? skip = 0);
    Task<int> GetProductCountCategoryAsync(int businessId, int categoryId);
    Task<int> GetProductCountAsync(int businessId);
    Task<Product?> GetProductByIdAsync(int productId, int businessId);
    Task<Product> UpdateProductAsync(Product product);
    Task<bool> DeleteProductAsync(int productId, int businessId);
}
