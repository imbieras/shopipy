using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shopipy.CategoryManagement.DTOs;
using Shopipy.CategoryManagement.Services;
using Shopipy.Persistence.Models;

namespace Shopipy.CategoryManagement.Controllers;

[ApiController]
[Route("[controller]/{businessId}")]
public class CategoryController(CategoryService categoryService, IMapper mapper) : ControllerBase
{
    [HttpGet("/categories")]
    public async Task<IActionResult> GetAllCategories(int businessId)
    {
        var categories = await categoryService.GetAllCategoriesInBusinessAsync(businessId);
        var responseDtos = mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        return Ok(responseDtos);
    }

    [HttpGet("/categories/{id}")]
    public async Task<IActionResult> GetCategoryById(int businessId, int id)
    {
        var category = await categoryService.GetCategoryByIdInBusinessAsync(businessId, id);
        if (category == null) return NotFound();

        var responseDto = mapper.Map<CategoryResponseDto>(category);
        return Ok(responseDto);
    }

    [HttpPost("/categories")]
    public async Task<IActionResult> CreateCategory(int businessId, CategoryRequestDto request)
    {
        var category = mapper.Map<Category>(request);
        category.BusinessId = businessId; 

        var createdCategory = await categoryService.CreateCategoryAsync(category);

        var responseDto = mapper.Map<CategoryResponseDto>(createdCategory);
        return CreatedAtAction(
            nameof(GetCategoryById),
            new { businessId = businessId, id = createdCategory.CategoryId },
            responseDto);
    }

    [HttpPut("/categories/{id}")]
    public async Task<IActionResult> UpdateCategory(int businessId, int id, CategoryRequestDto request)
    {
        var existingCategory = await categoryService.GetCategoryByIdInBusinessAsync(businessId, id);
        if (existingCategory == null) return NotFound();

        mapper.Map(request, existingCategory);
        var updatedCategory = await categoryService.UpdateCategoryAsync(existingCategory);

        var responseDto = mapper.Map<CategoryResponseDto>(updatedCategory);
        return Ok(responseDto);
    }

    [HttpDelete("/categories/{id}")]
    public async Task<IActionResult> DeleteCategory(int businessId, int id)
    {
        var existingCategory = await categoryService.GetCategoryByIdInBusinessAsync(businessId, id);
        if (existingCategory == null) return NotFound();

        var success = await categoryService.DeleteCategoryAsync(id);
        if (!success) return BadRequest("Failed to delete category.");
        return NoContent();
    }
}
