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

    [HttpGet]
    public async Task<IActionResult> GetAllCategories()
    {
        var categories = await _categoryService.GetAllCategoriesAsync();
        var responseDtos = _mapper.Map<IEnumerable<CategoryResponseDto>>(categories);
        return Ok(responseDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var category = await _categoryService.GetCategoryByIdAsync(id);
        if (category == null) return NotFound();

        var responseDto = _mapper.Map<CategoryResponseDto>(category);
        return Ok(responseDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateCategory(CategoryRequestDto request)
    {
        var category = _mapper.Map<Category>(request);
        var createdCategory = await _categoryService.CreateCategoryAsync(category);

        var responseDto = _mapper.Map<CategoryResponseDto>(createdCategory);
        return CreatedAtAction(nameof(GetCategoryById), new { id = createdCategory.CategoryId }, responseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCategory(int id, CategoryRequestDto request)
    {
        var existingCategory = await _categoryService.GetCategoryByIdAsync(id);
        if (existingCategory == null) return NotFound();

        _mapper.Map(request, existingCategory);
        var updatedCategory = await _categoryService.UpdateCategoryAsync(existingCategory);

        var responseDto = _mapper.Map<CategoryResponseDto>(updatedCategory);
        return Ok(responseDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var success = await _categoryService.DeleteCategoryAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}
