using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.BusinessManagement.DTOs;
using Shopipy.Persistence.Models;
using Shopipy.Shared;
using Shopipy.Shared.Services;

namespace Shopipy.BusinessManagement.Controllers;

[ApiController]
[Route("businesses")]
[Authorize(Policy = AuthorizationPolicies.RequireSuperAdmin)]
public class BusinessController(IBusinessService businessService, IMapper mapper, ILogger<BusinessController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBusinesses()
    {
        var businesses = await businessService.GetAllBusinessesAsync();
        var responseDtos = mapper.Map<IEnumerable<BusinessResponseDto>>(businesses);
        return Ok(responseDtos);
    }

    [HttpGet("{businessId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
    public async Task<IActionResult> GetBusiness(int businessId)
    {
        var business = await businessService.GetBusinessByIdAsync(businessId);
        if (business == null)
        {
            logger.LogWarning("Business with ID {BusinessId} not found.", businessId);
            return NotFound();
        }

        var responseDto = mapper.Map<BusinessResponseDto>(business);
        return Ok(responseDto);
    }
    [HttpPost]
    public async Task<IActionResult> CreateBusiness(BusinessRequestDto request)
    {
        var business = mapper.Map<Business>(request);
        var createdBusiness = await businessService.CreateBusinessAsync(business);
        var responseDto = mapper.Map<BusinessResponseDto>(createdBusiness);
        return CreatedAtAction(nameof(GetBusiness), new { id = createdBusiness.BusinessId }, responseDto);
    }

    [HttpPut("{businessId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
    public async Task<IActionResult> UpdateBusiness(int businessId, BusinessRequestDto request)
    {
        var existingBusiness = await businessService.GetBusinessByIdAsync(businessId);
        if (existingBusiness == null)
        {
            logger.LogWarning("Business with ID {BusinessId} not found.", businessId);
            return NotFound();
        }

        mapper.Map(request, existingBusiness);
        existingBusiness.UpdatedAt = DateTime.UtcNow;// Update the timestamp

        var updatedBusiness = await businessService.UpdateBusinessAsync(existingBusiness);
        var responseDto = mapper.Map<BusinessResponseDto>(updatedBusiness);

        return Ok(responseDto);
    }

    [HttpDelete("{businessId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
    public async Task<IActionResult> DeleteBusiness(int businessId)
    {
        var success = await businessService.DeleteBusinessAsync(businessId);

        if (success)
        {
            return NoContent();
        }

        logger.LogWarning("Business with ID {BusinessId} not found.", businessId);
        return NotFound();
    }
}