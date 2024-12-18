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
public class ProductController(IProductService productService, ICategoryService categoryService, IBusinessService businessService, IMapper mapper, ILogger<ProductController> logger) : ControllerBase
{
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
        var product = await productService.GetProductByIdInBusinessAsync(productId, businessId);
        if (product != null)
        {
            return Ok(mapper.Map<ProductResponseDto>(product));
        }

        logger.LogWarning("Product ID {ProductId} not found for Business ID {BusinessId}.", productId, businessId);
        return NotFound();
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> CreateProduct(ProductRequestDto dto, int businessId)
    {
        var business = await businessService.GetBusinessByIdAsync(businessId);
        if (business == null)
        {
            logger.LogWarning("Business with ID {BusinessId} not found for product creation.", businessId);
            return NotFound();
        }

        var categoryExists = await categoryService.GetCategoryByIdAsync(dto.CategoryId);
        if (categoryExists == null)
        {
            logger.LogWarning("Category ID {CategoryId} does not exist.", dto.CategoryId);
            return NotFound("Category does not exist");
        }

        var product = mapper.Map<Product>(dto);

        var createdProduct = await productService.CreateProductAsync(product, businessId);

        return CreatedAtAction(nameof(GetProductById), new { businessId, productId = createdProduct.ProductId }, mapper.Map<ProductResponseDto>(createdProduct));
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

        var existingProduct = await productService.GetProductByIdInBusinessAsync(productId, businessId);

        if (existingProduct == null)
        {
            logger.LogWarning("Product ID {ProductId} not found for Business ID {BusinessId}.", productId, businessId);
            return NotFound();
        }

        mapper.Map(dto, existingProduct);

        var updatedProduct = await productService.UpdateProductAsync(existingProduct);

        return Ok(mapper.Map<ProductResponseDto>(updatedProduct));
    }

    [HttpDelete("{productId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteProduct(int productId, int businessId)
    {
        var existingProduct = await productService.GetProductByIdInBusinessAsync(productId, businessId);
        if (existingProduct == null)
        {
            logger.LogWarning("Product ID {ProductId} not found in Business ID {BusinessId}.", productId, businessId);
            return NotFound();
        }

        var success = await productService.DeleteProductAsync(productId, businessId);
        if (success)
        {
            return NoContent();
        }

        logger.LogWarning("Product ID {ProductId} not found for Business ID {BusinessId}.", productId, businessId);
        return NotFound();
    }
}