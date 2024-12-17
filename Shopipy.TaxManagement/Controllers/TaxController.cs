using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.Persistence.Models;
using Shopipy.Shared;
using Shopipy.Shared.DTOs;
using Shopipy.Shared.Services;
using Shopipy.TaxManagement.DTOs;

namespace Shopipy.TaxManagement.Controllers;

[ApiController]
[Route("/businesses/{businessId:int}/taxrates/")]
[Tags("Tax")]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class TaxManagementController(ITaxService taxService, ICategoryService categoryService, IMapper mapper, ILogger logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTaxRates(int businessId, [FromQuery] int? top = null, [FromQuery] int? skip = null)
    {
        var taxRates = await taxService.GetAllTaxRatesByBusinessAsync(businessId, top, skip);
        var count = await taxService.GetTaxRateCountAsync(businessId);

        return Ok(new PaginationResultDto<TaxRateResponseDto> { Data = taxRates.Select(mapper.Map<TaxRateResponseDto>), Count = count });
    }

    [HttpGet("{taxId:int}")]
    public async Task<IActionResult> GetTaxRate(int businessId, int taxId)
    {
        var taxRate = await taxService.GetTaxRateByIdAndBusinessAsync(taxId, businessId);
        if (taxRate == null)
        {
            logger.LogWarning("Tax Rate {taxId} not found for business {BusinessId}.", taxId, businessId);
            return NotFound();
        }

        var responseDto = mapper.Map<TaxRateResponseDto>(taxRate);
        return Ok(responseDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaxRate(int businessId, TaxRateRequestDto request)
    {
        var taxRate = mapper.Map<TaxRate>(request);
        taxRate.BusinessId = businessId;

        var category = await categoryService.GetCategoryByIdAsync(taxRate.CategoryId);

        if (category == null)
        {
            logger.LogWarning("Category with ID {CategoryId} not found for discount creation.", taxRate.CategoryId);
            return NotFound();
        }

        var createdTaxRate = await taxService.AddTaxRateAsync(taxRate);
        var responseDto = mapper.Map<TaxRateResponseDto>(createdTaxRate);

        return CreatedAtAction(nameof(GetTaxRate), new { businessId, taxId = createdTaxRate.TaxRateId }, responseDto);
    }

    [HttpPut("{taxId:int}")]
    public async Task<IActionResult> UpdateTaxRate(
        int businessId,
        int taxId,
        [FromBody] DateTime? effectiveTo
    )
    {
        var existingTaxRate = await taxService.GetTaxRateByIdAndBusinessAsync(taxId, businessId);
        if (existingTaxRate == null)
        {
            logger.LogWarning("Attempted to update non-existent tax rate {TaxId} for business {BusinessId}.", taxId, businessId);
            return NotFound();
        }

        var updatedTaxRate = await taxService.UpdateTaxRateAsync(existingTaxRate, effectiveTo);

        var responseDto = mapper.Map<TaxRateResponseDto>(updatedTaxRate);
        return Ok(responseDto);
    }
    [HttpDelete("{taxId:int}")]
    public async Task<IActionResult> DeleteTaxRate(int businessId, int taxId)
    {
        var taxRate = await taxService.GetTaxRateByIdAndBusinessAsync(taxId, businessId);
        if (taxRate == null)
        {
            logger.LogWarning("Attempted to delete non-existent tax rate {TaxId} for business {BusinessId}.", taxId, businessId);
            return NotFound();
        }

        var success = await taxService.DeleteTaxRateAsync(taxId);
        if (success)
        {
            return NoContent();
        }

        logger.LogError("Failed to delete tax rate {TaxId} for business {BusinessId}.", taxId, businessId);
        return NotFound();
    }
}