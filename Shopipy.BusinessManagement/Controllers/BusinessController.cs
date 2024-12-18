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
    public async Task<ActionResult<IEnumerable<BusinessResponseDto>>> GetAllBusinesses()
    {
        var businesses = await businessService.GetAllBusinessesAsync();

        return Ok(businesses.Select(mapper.Map<BusinessResponseDto>));
    }

    [HttpGet("{businessId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
    public async Task<IActionResult> GetBusinessById(int businessId)
    {
        var business = await businessService.GetBusinessByIdAsync(businessId);
        if (business != null)
        {
            return Ok(mapper.Map<BusinessResponseDto>(business));
        }

        logger.LogWarning("Business with ID {BusinessId} not found.", businessId);
        return NotFound();
    }
    
    [HttpPost]
    public async Task<IActionResult> CreateBusiness([FromBody] BusinessRequestDto request)
    {
        var business = mapper.Map<Business>(request);
        var createdBusiness = await businessService.CreateBusinessAsync(business);
        return CreatedAtAction(nameof(GetBusinessById), new { businessId = createdBusiness.BusinessId }, mapper.Map<BusinessResponseDto>(createdBusiness));
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

        var updatedBusiness = await businessService.UpdateBusinessAsync(existingBusiness);

        return Ok(mapper.Map<BusinessResponseDto>(updatedBusiness));
    }

    [HttpDelete("{businessId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
    public async Task<IActionResult> DeleteBusiness(int businessId)
    {
        var business = await businessService.GetBusinessByIdAsync(businessId);
        if (business == null)
        {
            logger.LogWarning("Business with ID {BusinessId} not found.", businessId);
            return NotFound();
        }
        
        var success = await businessService.DeleteBusinessAsync(businessId);

        if (success)
        {
            return NoContent();
        }

        logger.LogWarning("Business with ID {BusinessId} not found.", businessId);
        return NotFound();
    }
}