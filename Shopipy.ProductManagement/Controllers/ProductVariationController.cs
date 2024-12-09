using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.ProductManagement.DTOs;
using Shopipy.Persistence.Models;
using Shopipy.Shared;
using Shopipy.Shared.Services;
using Shopipy.Shared.DTOs;

namespace Shopipy.ProductManagement.Controllers;

[Authorize]
[Route("businesses/{businessId}/products/{productId}/variations")]
[ApiController]
public class ProductVariationController(IProductVariationService _variationService, IMapper _mapper) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<ProductVariationResponseDTO>> CreateVariation(int businessId, int productId, ProductVariationRequestDTO dto)
    {
        var variation = _mapper.Map<ProductVariation>(dto);

        var createdVariation = await _variationService.CreateVariationAsync(variation, productId, businessId);
        var variationResponseDTO = _mapper.Map<ProductVariationResponseDTO>(createdVariation);

        return CreatedAtAction(nameof(GetVariationById),
            new { businessId, productId, variationId = createdVariation.VariationId },
            variationResponseDTO);       
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<ProductVariationResponseDTO>>> GetAllVariationsOfProductInBusiness(int businessId, int productId, [FromQuery] int? top = null, [FromQuery] int? skip = null)
    {
        var variations = await _variationService.GetAllVariationsOfProductInBusinessAsync(productId, businessId, top, skip);

        var count = await _variationService.GetVariationCountOfProductAsync(productId, businessId);

        return Ok(new PaginationResultDto<ProductVariationResponseDTO>
        {
            Data = variations.Select(_mapper.Map<ProductVariationResponseDTO>),
            Count = count
        });
    }

    [HttpGet("{variationId}")]
    public async Task<ActionResult<ProductVariationResponseDTO>> GetVariationById(int businessId, int productId, int variationId)
    {
        var variation = await _variationService.GetVariationByIdAsync(variationId, productId, businessId);
        if (variation == null) return NotFound();

        var variationResponseDTO = _mapper.Map<ProductVariationResponseDTO>(variation);
        return Ok(variationResponseDTO);
    }

    [HttpPut("{variationId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<ProductVariationResponseDTO>> UpdateVariation(int businessId, int productId, int variationId, ProductVariationRequestDTO dto)
    {
        var variation = await _variationService.GetVariationByIdAsync(variationId, productId, businessId);

        if (variation == null) return NotFound();

        _mapper.Map(dto, variation);

        var updatedVariation = await _variationService.UpdateVariationAsync(variation);
        var variationResponseDTO = _mapper.Map<ProductVariationResponseDTO>(updatedVariation);

        return Ok(variationResponseDTO);
    }

    [HttpDelete("{variationId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteVariationAsync(int businessId, int productId, int variationId)
    {
        var success = await _variationService.DeleteVariationAsync(variationId, productId, businessId);
        if (!success) return NotFound();

        return NoContent();
    }
}
