using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.CategoryManagement.DTOs;
using Shopipy.Persistence.Models;
using Shopipy.Shared;
using Shopipy.Shared.Services;

namespace Shopipy.CategoryManagement.Controllers;

[ApiController]
[Route("businesses/{businessId}/categories")]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class CategoryController(ICategoryService categoryService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCategories(int businessId)
    {
        var categories = await categoryService.GetAllCategoriesInBusinessAsync(businessId);
        var responseDtos = mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        return Ok(responseDtos);
    }

    [HttpGet("{categoryId}")]
    public async Task<IActionResult> GetCategoryById(int businessId, int categoryId)
    {
        var category = await categoryService.GetCategoryByIdInBusinessAsync(businessId, categoryId);
        if (category == null) return NotFound();

        var responseDto = mapper.Map<CategoryResponseDto>(category);
        return Ok(responseDto);
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> CreateCategory(int businessId, CategoryRequestDto request)
    {
        var category = mapper.Map<Category>(request);
        category.BusinessId = businessId; 

        var createdCategory = await categoryService.CreateCategoryAsync(category);

        var responseDto = mapper.Map<CategoryResponseDto>(createdCategory);
        return CreatedAtAction(
            nameof(GetCategoryById),
            new { businessId = businessId, categoryId = createdCategory.CategoryId },
            responseDto);
    }

    [HttpPut("{categoryId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> UpdateCategory(int businessId, int categoryId, CategoryRequestDto request)
    {
        var existingCategory = await categoryService.GetCategoryByIdInBusinessAsync(businessId, categoryId);
        if (existingCategory == null) return NotFound();

        mapper.Map(request, existingCategory);
        var updatedCategory = await categoryService.UpdateCategoryAsync(existingCategory);

        var responseDto = mapper.Map<CategoryResponseDto>(updatedCategory);
        return Ok(responseDto);
    }

    [HttpDelete("{categoryId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteCategory(int businessId, int categoryId)
    {
        var existingCategory = await categoryService.GetCategoryByIdInBusinessAsync(businessId, categoryId);
        if (existingCategory == null) return NotFound();

        var success = await categoryService.DeleteCategoryAsync(categoryId);
        if (!success) return BadRequest("Failed to delete category.");
        return NoContent();
    }
}
