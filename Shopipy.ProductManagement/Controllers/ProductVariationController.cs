﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.ProductManagement.DTOs;
using Shopipy.Persistence.Models;
using Shopipy.Shared;
using Shopipy.Shared.Services;
using Shopipy.Shared.DTOs;

namespace Shopipy.ProductManagement.Controllers;

[Authorize]
[Route("businesses/{businessId:int}/products/{productId:int}/variations")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class ProductVariationController(IProductVariationService variationService, IMapper mapper, ILogger<ProductVariationController> logger) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<ProductVariationResponseDto>> CreateVariation(int businessId, int productId, ProductVariationRequestDto dto)
    {
        var variation = mapper.Map<ProductVariation>(dto);

        var createdVariation = await variationService.CreateVariationAsync(variation, productId, businessId);
        var variationResponseDto = mapper.Map<ProductVariationResponseDto>(createdVariation);

        return CreatedAtAction(nameof(GetVariationById),
        new { businessId, productId, variationId = createdVariation.VariationId },
        variationResponseDto);
    }

    [HttpGet]
    public async Task<ActionResult<PaginationResultDto<ProductVariationResponseDto>>> GetAllVariationsOfProductInBusiness(int businessId, int productId, [FromQuery] int? top = null, [FromQuery] int? skip = null)
    {
        var variations = await variationService.GetAllVariationsOfProductInBusinessAsync(productId, businessId, top, skip);

        var count = await variationService.GetVariationCountOfProductAsync(productId, businessId);

        return Ok(new PaginationResultDto<ProductVariationResponseDto> { Data = variations.Select(mapper.Map<ProductVariationResponseDto>), Count = count });
    }

    [HttpGet("{variationId:int}")]
    public async Task<ActionResult<ProductVariationResponseDto>> GetVariationById(int businessId, int productId, int variationId)
    {
        var variation = await variationService.GetVariationByIdAsync(variationId, productId, businessId);
        if (variation == null)
        {
            logger.LogWarning("Variation ID {VariationId} not found for Product ID {ProductId} in Business ID {BusinessId}.", variationId, productId, businessId);
            return NotFound();
        }

        var variationResponseDto = mapper.Map<ProductVariationResponseDto>(variation);
        return Ok(variationResponseDto);
    }

    [HttpPut("{variationId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<ProductVariationResponseDto>> UpdateVariation(int businessId, int productId, int variationId, ProductVariationRequestDto dto)
    {
        var variation = await variationService.GetVariationByIdAsync(variationId, productId, businessId);

        if (variation == null)
        {
            logger.LogWarning("Variation ID {VariationId} not found for Product ID {ProductId} in Business ID {BusinessId}.", variationId, productId, businessId);
            return NotFound();
        }

        mapper.Map(dto, variation);

        var updatedVariation = await variationService.UpdateVariationAsync(variation);
        var variationResponseDto = mapper.Map<ProductVariationResponseDto>(updatedVariation);

        return Ok(variationResponseDto);
    }

    [HttpDelete("{variationId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteVariationAsync(int businessId, int productId, int variationId)
    {
        var success = await variationService.DeleteVariationAsync(variationId, productId, businessId);
        if (success)
        {
            return NoContent();
        }

        logger.LogWarning("Variation ID {VariationId} not found for Product ID {ProductId} in Business ID {BusinessId}.", variationId, productId, businessId);
        return NotFound();
    }
}