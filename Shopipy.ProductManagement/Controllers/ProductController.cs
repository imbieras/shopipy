using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.Persistence.Models;
using Shopipy.ProductManagement.DTOs;
using Shopipy.Shared;
using Shopipy.Shared.Services;

namespace Shopipy.ProductManagement.Controllers;

[Route("{businessId}/products")]
[ApiController]
public class ProductController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly IMapper _mapper;

    public ProductController(IProductService productService, IMapper mapper)
    {
        _productService = productService;
        _mapper = mapper;
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<ProductResponseDTO>> CreateProductAsync(ProductRequestDTO dto, int businessId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Invalid data provided", details = ModelState });
        }

        var product = _mapper.Map<Product>(dto);
        var createdProduct = await _productService.CreateProductAsync(product, businessId);

        var productResponseDTO = _mapper.Map<ProductResponseDTO>(createdProduct);
        return CreatedAtAction(nameof(GetProductByIdAsync), new { productId = createdProduct.ProductId }, productResponseDTO);
    }

    [HttpGet("{productId}")]
    public async Task<ActionResult<ProductResponseDTO>> GetProductByIdAsync(int productId, int businessId)
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
    public async Task<ActionResult<ProductResponseDTO>> UpdateProductAsync(int productId, ProductRequestDTO dto, int businessId)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(new { error = "Invalid data provided", details = ModelState });
        }

        var existingProduct = await _productService.GetProductByIdAsync(productId, businessId);

        if (existingProduct == null) return NotFound();

        _mapper.Map(dto, existingProduct);
        existingProduct.UpdatedAt = DateTime.UtcNow;

        var updatedProduct = await _productService.UpdateProductAsync(productId, existingProduct, businessId);
        var productResponseDTO = _mapper.Map<ProductResponseDTO>(updatedProduct);

        return Ok(productResponseDTO);
    }

    [HttpDelete("{productId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteProductAsync(int productId, int businessId)
    {
        var success = await _productService.DeleteProductAsync(productId, businessId);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }

}