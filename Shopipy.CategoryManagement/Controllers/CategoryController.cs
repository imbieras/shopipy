using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.CategoryManagement.DTOs;
using Shopipy.Persistence.Models;
using Shopipy.Shared;
using Shopipy.Shared.Services;

namespace Shopipy.CategoryManagement.Controllers;

[ApiController]
[Route("businesses/{businessId:int}/categories")]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class CategoryController(ICategoryService categoryService, IBusinessService businessService, IMapper mapper, ILogger<CategoryController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllCategories(int businessId)
    {
        var categories = await categoryService.GetAllCategoriesInBusinessAsync(businessId);
        var responseDtos = mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        return Ok(responseDtos);
    }

    [HttpGet("{categoryId:int}")]
    public async Task<IActionResult> GetCategoryById(int businessId, int categoryId)
    {
        var category = await categoryService.GetCategoryByIdInBusinessAsync(businessId, categoryId);
        if (category != null)
        {
            return Ok(mapper.Map<CategoryResponseDto>(category));
        }

        logger.LogWarning("Category with ID {CategoryId} in business {BusinessId} not found.", categoryId, businessId);
        return NotFound();
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> CreateCategory(int businessId, CategoryRequestDto request)
    {
        var business = await businessService.GetBusinessByIdAsync(businessId);
        if (business == null)
        {
            logger.LogWarning("Business with ID {BusinessId} not found for category creation.", businessId);
            return NotFound();
        }

        var existingCategory = await categoryService.GetAllCategoriesInBusinessAsync(businessId);

        if (existingCategory.Any(c => c.Name.Equals(request.Name, StringComparison.OrdinalIgnoreCase)))
        {
            logger.LogWarning("Category with {Name} already exists in business with ID {BusinessId}.", request.Name, businessId);
            return Conflict("A category with the same name already exists in this business.");
        }

        var category = mapper.Map<Category>(request);
        category.BusinessId = businessId;

        var createdCategory = await categoryService.CreateCategoryAsync(category);

        var responseDto = mapper.Map<CategoryResponseDto>(createdCategory);
        return CreatedAtAction(
        nameof(GetCategoryById),
        new { businessId, categoryId = createdCategory.CategoryId },
        responseDto);
    }

    [HttpPut("{categoryId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> UpdateCategory(int businessId, int categoryId, CategoryRequestDto request)
    {
        var existingCategory = await categoryService.GetCategoryByIdInBusinessAsync(businessId, categoryId);
        if (existingCategory == null)
        {
            logger.LogWarning("Category with ID {CategoryId} in business {BusinessId} not found for update.", categoryId, businessId);
            return NotFound();
        }

        mapper.Map(request, existingCategory);

        var updatedCategory = await categoryService.UpdateCategoryAsync(existingCategory);

        return Ok(mapper.Map<CategoryResponseDto>(updatedCategory));
    }

    [HttpDelete("{categoryId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteCategory(int businessId, int categoryId)
    {
        var existingCategory = await categoryService.GetCategoryByIdInBusinessAsync(businessId, categoryId);
        if (existingCategory == null)
        {
            logger.LogWarning("Category with ID {CategoryId} in business {BusinessId} not found for deletion.", categoryId, businessId);
            return NotFound();
        }

        var success = await categoryService.DeleteCategoryAsync(categoryId);

        if (success)
        {
            return NoContent();
        }

        logger.LogError("Failed to delete category with ID {CategoryId} in business {BusinessId}.", categoryId, businessId);
        return NotFound();
    }
}