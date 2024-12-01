using Shopipy.CategoryManagement.Services;
using Shopipy.Shared.Services;

namespace Shopipy.CategoryManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddCategoryManagement(this IServiceCollection categories)
    {
        categories.AddScoped<ICategoryService, CategoryService>();
        return categories;
    }
}