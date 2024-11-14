using Microsoft.AspNetCore.Mvc;
using Shopipy.BusinessManagement.DTOs;
using Shopipy.BusinessManagement.Services;
using Shopipy.Persistence.Models;

namespace Shopipy.BusinessManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class BusinessController : ControllerBase
{
    private readonly BusinessService _businessService;

    public BusinessController(BusinessService businessService)
    {
        _businessService = businessService;
    }
    private BusinessResponseDto ToResponseDto(Business business) // main candidate for automapper
    {
        return new BusinessResponseDto
        {
            BusinessId = business.BusinessId,
            Name = business.Name,
            Address = business.Address,
            Phone = business.Phone,
            Email = business.Email,
            VATNumber = business.VATNumber,
            BusinessType = business.BusinessType,
            CreatedAt = business.CreatedAt,
            UpdatedAt = business.UpdatedAt
        };
    }
    [HttpGet]
    public async Task<IActionResult> GetBusinesses()
    {
        var businesses = await _businessService.GetAllBusinessesAsync();
        var responseDtos = businesses.Select(ToResponseDto);
        return Ok(responseDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBusiness(int id)
    {
        var business = await _businessService.GetBusinessByIdAsync(id);
        if (business == null) return NotFound();

        var responseDto = ToResponseDto(business);
        return Ok(responseDto);
    }
    [HttpPost]
    public async Task<IActionResult> CreateBusiness(BusinessRequestDto request)
    {
        var business = new Business //add automapper here
        {
            Name = request.BusinessName,
            Address = request.BusinessAddress,
            VATNumber = request.BusinessVatNumber,
            Email = request.BusinessEmail,
            Phone = request.BusinessPhone,
            BusinessType = request.BusinessType,
        };   
        var createdBusiness = await _businessService.CreateBusinessAsync(business);
        return CreatedAtAction(nameof(GetBusiness), new { id = createdBusiness.BusinessId }, createdBusiness);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBusiness(int id, BusinessRequestDto request)
    {
        var existingBusiness = await _businessService.GetBusinessByIdAsync(id);
        if (existingBusiness == null)
        {
            return NotFound();
        }

        // Map fields
        existingBusiness.Name = request.BusinessName;
        existingBusiness.Address = request.BusinessAddress;
        existingBusiness.VATNumber = request.BusinessVatNumber;
        existingBusiness.Email = request.BusinessEmail;
        existingBusiness.Phone = request.BusinessPhone;
        existingBusiness.UpdatedAt = DateTime.UtcNow; // Update the timestamp

        var updatedBusiness = await _businessService.UpdateBusinessAsync(existingBusiness);

        return Ok(ToResponseDto(updatedBusiness));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBusiness(int id)
    {
        var success = await _businessService.DeleteBusinessAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}