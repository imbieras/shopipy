using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shopipy.BusinessManagement.DTOs;
using Shopipy.BusinessManagement.Services;
using Shopipy.Persistence.Models;

namespace Shopipy.BusinessManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class BusinessController(BusinessService businessService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetBusinesses()
    {
        var businesses = await businessService.GetAllBusinessesAsync();
        var responseDtos = mapper.Map<IEnumerable<BusinessResponseDto>>(businesses); 
        return Ok(responseDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBusiness(int id)
    {
        var business = await businessService.GetBusinessByIdAsync(id);
        if (business == null) return NotFound();

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

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBusiness(int id, BusinessRequestDto request)
    {
        var existingBusiness = await businessService.GetBusinessByIdAsync(id);
        if (existingBusiness == null) return NotFound();

        mapper.Map(request, existingBusiness); 
        existingBusiness.UpdatedAt = DateTime.UtcNow; // Update the timestamp

        var updatedBusiness = await businessService.UpdateBusinessAsync(existingBusiness);
        var responseDto = mapper.Map<BusinessResponseDto>(updatedBusiness);

        return Ok(responseDto);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBusiness(int id)
    {
        var success = await businessService.DeleteBusinessAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}