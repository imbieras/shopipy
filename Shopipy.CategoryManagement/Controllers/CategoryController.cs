using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shopipy.CategoryManagement.DTOs;
using Shopipy.CategoryManagement.Services;
using Shopipy.Persistence.Models;

namespace Shopipy.CategoryManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class CategoryController : ControllerBase
{
    private readonly CategoryService _categoryService;
    private readonly IMapper _mapper;

    public CategoryController(CategoryService categoryService, IMapper mapper)
    {
        _categoryService = categoryService;
        _mapper = mapper;
    }

    [HttpGet("{businessId}/categories")]
    public async Task<IActionResult> GetAllCategories(int businessId)
    {
        var categories = await _categoryService.GetAllCategoriesInBusinessAsync(businessId);
        var responseDtos = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        return Ok(responseDtos);
    }

    [HttpGet("{businessId}/categories/{id}")]
    public async Task<IActionResult> GetCategoryById(int businessId, int id)
    {
        var category = await _categoryService.GetCategoryByIdInBusinessAsync(businessId, id);
        if (category == null) return NotFound();

        var responseDto = _mapper.Map<CategoryResponseDto>(category);
        return Ok(responseDto);
    }

    [HttpPost("{businessId}/categories")]
    public async Task<IActionResult> CreateCategory(int businessId, CategoryRequestDto request)
    {
        var category = _mapper.Map<Category>(request);
        category.BusinessId = businessId; 

        var createdCategory = await _categoryService.CreateCategoryAsync(category);

        var responseDto = _mapper.Map<CategoryResponseDto>(createdCategory);
        return CreatedAtAction(
            nameof(GetCategoryById),
            new { businessId = businessId, id = createdCategory.CategoryId },
            responseDto);
    }

    [HttpPut("{businessId}/categories/{id}")]
    public async Task<IActionResult> UpdateCategory(int businessId, int id, CategoryRequestDto request)
    {
        var existingCategory = await _categoryService.GetCategoryByIdInBusinessAsync(businessId, id);
        if (existingCategory == null) return NotFound();

        _mapper.Map(request, existingCategory);
        var updatedCategory = await _categoryService.UpdateCategoryAsync(existingCategory);

        var responseDto = _mapper.Map<CategoryResponseDto>(updatedCategory);
        return Ok(responseDto);
    }

    [HttpDelete("{businessId}/categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int businessId, int id)
    {
        var existingCategory = await _categoryService.GetCategoryByIdInBusinessAsync(businessId, id);
        if (existingCategory == null) return NotFound();

        var success = await _categoryService.DeleteCategoryAsync(id);
        if (!success) return BadRequest("Failed to delete category.");
        return NoContent();
    }
}
