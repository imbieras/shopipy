using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface ICategoryService
{
    Task<IEnumerable<Category>> GetAllCategoriesAsync();
    Task<IEnumerable<Category>> GetAllCategoriesInBusinessAsync(int businessId);
    Task<Category> GetCategoryByIdAsync(int id);
    Task<Category> GetCategoryByIdInBusinessAsync(int businessId, int id);
    Task<Category> CreateCategoryAsync(Category category);
    Task<Category> UpdateCategoryAsync(Category category);
    Task<bool> DeleteCategoryAsync(int id);
}