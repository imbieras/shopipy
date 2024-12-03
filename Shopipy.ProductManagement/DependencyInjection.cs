using Shopipy.Shared.Services;
using Shopipy.ProductManagement.Services;

namespace Shopipy.ProductManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddProductManagement(this IServiceCollection products)
    {
        products.AddScoped<IProductService, ProductService>();
        products.AddScoped<IProductVariationService, ProductVariationService>();
        return products;
    }
}
