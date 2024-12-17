using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.Persistence.Models;
using Shopipy.ProductManagement.DTOs;
using Shopipy.Shared;
using Shopipy.Shared.DTOs;
using Shopipy.Shared.Services;

namespace Shopipy.ProductManagement.Controllers;

[Authorize]
[Route("businesses/{businessId:int}/products")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class ProductController(IProductService productService, ICategoryService categoryService, IMapper mapper, ILogger<ProductController> logger) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> CreateProduct(ProductRequestDto dto, int businessId)
    {
        var categoryExists = await categoryService.GetCategoryByIdAsync(dto.CategoryId);
        if (categoryExists == null)
        {
            logger.LogWarning("Category ID {CategoryId} does not exist.", dto.CategoryId);
            return NotFound("Category does not exist");
        }

        var product = mapper.Map<Product>(dto);

        var createdProduct = await productService.CreateProductAsync(product, businessId);

        var productResponseDto = mapper.Map<ProductResponseDto>(createdProduct);

        return CreatedAtAction(nameof(GetProductById), new { businessId, productId = createdProduct.ProductId }, productResponseDto);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts(int businessId, [FromQuery] int? top = null, [FromQuery] int? skip = null, [FromQuery] int? categoryId = null)
    {
        if (categoryId.HasValue)
        {
            var category = await categoryService.GetCategoryByIdAsync(categoryId.Value);
            if (category == null)
            {
                logger.LogWarning("Category ID {CategoryId} does not exist for Business ID {BusinessId}.", categoryId, businessId);
                return NotFound("Category does not exist");
            }
        }

        var products = await productService.GetAllProductsAsync(businessId, categoryId, top, skip);
        var count = await productService.GetProductCountAsync(businessId, categoryId);

        return Ok(new PaginationResultDto<ProductResponseDto> { Data = products.Select(mapper.Map<ProductResponseDto>), Count = count });
    }

    [HttpGet("{productId:int}")]
    public async Task<ActionResult<ProductResponseDto>> GetProductById(int businessId, int productId)
    {
        var product = await productService.GetProductByIdAsync(productId, businessId);
        if (product == null)
        {
            logger.LogWarning("Product ID {ProductId} not found for Business ID {BusinessId}.", productId, businessId);
            return NotFound();
        }

        var productResponseDto = mapper.Map<ProductResponseDto>(product);
        return Ok(productResponseDto);
    }

    [HttpPut("{productId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<ProductResponseDto>> UpdateProduct(int productId, ProductRequestDto dto, int businessId)
    {
        var categoryExists = await categoryService.GetCategoryByIdAsync(dto.CategoryId);
        if (categoryExists == null)
        {
            logger.LogWarning("Category ID {CategoryId} does not exist.", dto.CategoryId);
            return NotFound("Category does not exist");
        }

        var existingProduct = await productService.GetProductByIdAsync(productId, businessId);

        if (existingProduct == null)
        {
            logger.LogWarning("Product ID {ProductId} not found for Business ID {BusinessId}.", productId, businessId);
            return NotFound();
        }

        mapper.Map(dto, existingProduct);

        var updatedProduct = await productService.UpdateProductAsync(existingProduct);
        var productResponseDto = mapper.Map<ProductResponseDto>(updatedProduct);

        return Ok(productResponseDto);
    }

    [HttpDelete("{productId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteProduct(int productId, int businessId)
    {
        var success = await productService.DeleteProductAsync(productId, businessId);
        if (success)
        {
            return NoContent();
        }

        logger.LogWarning("Product ID {ProductId} not found for Business ID {BusinessId}.", productId, businessId);
        return NotFound();
    }
}