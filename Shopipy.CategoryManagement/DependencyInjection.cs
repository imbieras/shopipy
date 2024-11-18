using Shopipy.CategoryManagement.Services;

namespace Shopipy.CategoryManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddCategoryManagement(this IServiceCollection categories)
    {
        categories.AddScoped<CategoryService>();
        return categories;
    }
}