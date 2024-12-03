using Shopipy.Shared.Services;
using Shopipy.ProductManagement.Services;

namespace Shopipy.ProductManagement
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddCategoryManagement(this IServiceCollection categories)
        {
            categories.AddScoped<IProductService, ProductService>();
            return categories;
        }
    }
}
