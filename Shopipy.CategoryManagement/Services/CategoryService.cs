using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;

namespace Shopipy.CategoryManagement.Services;

public class CategoryService
{
    private readonly IGenericRepository<Category> _categoryRepository;

    public CategoryService(IGenericRepository<Category> categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
    {
        return await _categoryRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Category>> GetAllCategoriesInBusinessAsync(int businessId)
    {
        return await _categoryRepository.GetAllByConditionAsync(c => c.BusinessId == businessId);
    }

    public async Task<Category> GetCategoryByIdAsync(int id)
    {
        return await _categoryRepository.GetByIdAsync(id);
    }

    public async Task<Category> GetCategoryByIdInBusinessAsync(int businessId, int id)
    {
        return await _categoryRepository.GetByConditionAsync(c => c.BusinessId == businessId && c.CategoryId == id);
    }
    
    public async Task<Category> CreateCategoryAsync(Category category)
    {
        return await _categoryRepository.AddAsync(category);
    }

    public async Task<Category> UpdateCategoryAsync(Category category)
    {
        return await _categoryRepository.UpdateAsync(category);
    }

    public async Task<bool> DeleteCategoryAsync(int id)
    {
        return await _categoryRepository.DeleteAsync(id);
    }
}