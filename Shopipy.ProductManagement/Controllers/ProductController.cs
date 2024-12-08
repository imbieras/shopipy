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
[Route("businesses/{businessId}/products")]
[ApiController]
public class ProductController(IProductService _productService, IMapper _mapper) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> CreateProduct(ProductRequestDTO dto, int businessId)
    {
        var product = _mapper.Map<Product>(dto);

        var createdProduct = await _productService.CreateProductAsync(product, businessId);

        var productResponseDTO = _mapper.Map<ProductResponseDTO>(createdProduct);

        return CreatedAtAction(nameof(GetProductById), new { businessId, productId = createdProduct.ProductId }, productResponseDTO);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts(int businessId, [FromQuery] int? top = null, [FromQuery] int? skip = null)
    {
        var products = await _productService.GetAllProductsOfBusinessAsync(businessId, top, skip);
        var count = await _productService.GetProductCountAsync(businessId);

        return Ok(new PaginationResultDto<ProductResponseDTO>
        {
            Data = products.Select(_mapper.Map<ProductResponseDTO>),
            Count = count
        });
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductResponseDTO>> GetProductById(int businessId, int productId)
    {
        var product = await _productService.GetProductByIdAsync(productId, businessId);
        if (product == null)
        {
            return NotFound();
        }

        var productResponseDTO = _mapper.Map<ProductResponseDTO>(product);
        return Ok(productResponseDTO);
    }

    [HttpPut("{productId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<ProductResponseDTO>> UpdateProduct(int productId, ProductRequestDTO dto, int businessId)
    {
        var existingProduct = await _productService.GetProductByIdAsync(productId, businessId);

        if (existingProduct == null) return NotFound();

        _mapper.Map(dto, existingProduct);

        var updatedProduct = await _productService.UpdateProductAsync(existingProduct);
        var productResponseDTO = _mapper.Map<ProductResponseDTO>(updatedProduct);

        return Ok(productResponseDTO);
    }

    [HttpDelete("{productId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteProduct(int productId, int businessId)
    {
        var success = await _productService.DeleteProductAsync(productId, businessId);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

}