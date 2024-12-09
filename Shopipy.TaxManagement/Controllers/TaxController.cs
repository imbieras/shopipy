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
[Route("Tax/{businessId}")]
[Tags("Tax")]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
public class TaxManagementController(ITaxService taxService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTaxRates(int businessId, [FromQuery] int? top = null, [FromQuery] int? skip = null)
    {
        var taxRates = await taxService.GetAllTaxRatesByBusinessAsync(businessId, top, skip);
        var count = await taxService.GetTaxRateCountAsync(businessId);

        return Ok(new PaginationResultDto<TaxRateResponseDto>
        {
            Data = taxRates.Select(mapper.Map<TaxRateResponseDto>),
            Count = count
        });
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetTaxRate(int businessId, int id)
    {
        var taxRate = await taxService.GetTaxRateByIdAndBusinessAsync(id, businessId); 
        if (taxRate == null) return NotFound();

        var responseDto = mapper.Map<TaxRateResponseDto>(taxRate);
        return Ok(responseDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateTaxRate(int businessId, TaxRateRequestDto request)
    {
        var taxRate = mapper.Map<TaxRate>(request);
        taxRate.BusinessId = businessId; // Assign the businessId
        var createdTaxRate = await taxService.AddTaxRateAsync(taxRate);
        var responseDto = mapper.Map<TaxRateResponseDto>(createdTaxRate);

        return CreatedAtAction(nameof(GetTaxRate), new { businessId, id = createdTaxRate.TaxRateId }, responseDto);
    }

    [HttpPut("{taxId}")]
    public async Task<IActionResult> UpdateTaxRate(
        int businessId,
        int taxId,
        [FromBody] DateTime? effectiveTo)
    {
        var existingTaxRate = await taxService.GetTaxRateByIdAndBusinessAsync(taxId, businessId);
        if (existingTaxRate == null) return NotFound();

        var updatedTaxRate = await taxService.UpdateTaxRateAsync(existingTaxRate, effectiveTo);

        var responseDto = mapper.Map<TaxRateResponseDto>(updatedTaxRate);
        return Ok(responseDto);
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteTaxRate(int businessId, int id)
    {
        var taxRate = await taxService.GetTaxRateByIdAndBusinessAsync(id, businessId); 
        if (taxRate == null) return NotFound();

        var success = await taxService.DeleteTaxRateAsync(id);
        if (!success) return NotFound();

        return NoContent();
    }
}
