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
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class TaxManagementController(ITaxService taxService, ICategoryService categoryService, IBusinessService businessService, IMapper mapper, ILogger<TaxManagementController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<TaxRateResponseDto>>> GetTaxRates(int businessId)
    {
        var taxRates = await taxService.GetAllTaxRatesByBusinessAsync(businessId);

        return Ok(taxRates.Select(mapper.Map<TaxRateResponseDto>));
    }

    [HttpGet("{taxId:int}")]
    public async Task<IActionResult> GetTaxRateById(int businessId, int taxId)
    {
        var taxRate = await taxService.GetTaxRateByIdAndBusinessAsync(taxId, businessId);
        if (taxRate != null)
        {
            return Ok(mapper.Map<TaxRateResponseDto>(taxRate));
        }

        logger.LogWarning("Tax Rate {taxId} not found for business {BusinessId}.", taxId, businessId);
        return NotFound();

    }

    
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> CreateTaxRate(int businessId, TaxRateRequestDto request)
    {
        var business = await businessService.GetBusinessByIdAsync(businessId);
        if (business == null)
        {
            logger.LogWarning("Business with ID {BusinessId} not found for tax rate creation.", businessId);
            return NotFound();
        }

        var taxRate = mapper.Map<TaxRate>(request);
        taxRate.BusinessId = businessId;

        var category = await categoryService.GetCategoryByIdAsync(taxRate.CategoryId);

        if (category == null)
        {
            logger.LogWarning("Category with ID {CategoryId} not found for tax rate creation.", taxRate.CategoryId);
            return NotFound();
        }

        var createdTaxRate = await taxService.CreateTaxRateAsync(taxRate);

        return CreatedAtAction(nameof(GetTaxRateById), new { businessId, taxId = createdTaxRate.TaxRateId }, mapper.Map<TaxRateResponseDto>(createdTaxRate));
    }

    [HttpPut("{taxId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> UpdateTaxRate(
        int businessId,
        int taxId,
        [FromBody] DateTime? effectiveTo
    )
    {
        var existingTaxRate = await taxService.GetTaxRateByIdAndBusinessAsync(taxId, businessId);
        if (existingTaxRate == null)
        {
            logger.LogWarning("Tax rate with ID {TaxId} in business {BusinessId} not found.", taxId, businessId);
            return NotFound();
        }

        var updatedTaxRate = await taxService.UpdateTaxRateAsync(existingTaxRate, effectiveTo);

        var responseDto = mapper.Map<TaxRateResponseDto>(updatedTaxRate);
        return Ok(responseDto);
    }
    
    [HttpDelete("{taxId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteTaxRate(int businessId, int taxId)
    {
        var taxRate = await taxService.GetTaxRateByIdAndBusinessAsync(taxId, businessId);
        if (taxRate == null)
        {
            logger.LogWarning("Tax rate with ID {TaxId} in business {BusinessId} not found.", taxId, businessId);
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