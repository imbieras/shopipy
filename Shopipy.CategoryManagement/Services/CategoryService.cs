using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.CategoryManagement.Services;

public class CategoryService(IGenericRepository<Category> categoryRepository) : ICategoryService
{
    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await categoryRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesInBusinessAsync(int businessId)
    {
        return await categoryRepository.GetAllByConditionAsync(c => c.BusinessId == businessId);
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        return await categoryRepository.GetByIdAsync(id);
    }

    public async Task<Category> GetCategoryByIdInBusinessAsync(int businessId, int id)
    {
        return await categoryRepository.GetByConditionAsync(c => c.BusinessId == businessId && c.CategoryId == id);
    }
    
    public async Task<Category> CreateCategoryAsync(Category category)
    {
        return await categoryRepository.AddAsync(category);
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        return await categoryRepository.UpdateAsync(category);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        return await categoryRepository.DeleteAsync(id);
    }
}