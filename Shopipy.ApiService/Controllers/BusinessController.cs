using Microsoft.AspNetCore.Mvc;
using Shopipy.ApiService.Models;
using Shopipy.ApiService.Services;

namespace Shopipy.ApiService.Controllers;

[ApiController]
[Route("[controller]")]
public class BusinessController : ControllerBase
{
    private readonly BusinessService _businessService;

    public BusinessController(BusinessService businessService)
    {
        _businessService = businessService;
    }

    [HttpGet]
    public async Task<IActionResult> GetBusinesses()
    {
        var businesses = await _businessService.GetAllBusinessesAsync();
        return Ok(businesses);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetBusiness(int id)
    {
        var business = await _businessService.GetBusinessByIdAsync(id);
        if (business == null) return NotFound();
        return Ok(business);
    }

    [HttpPost]
    public async Task<IActionResult> CreateBusiness(Business business)
    {
        var createdBusiness = await _businessService.CreateBusinessAsync(business);
        return CreatedAtAction(nameof(GetBusiness), new { id = createdBusiness.BusinessId }, createdBusiness);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateBusiness(int id, Business business)
    {
        if (id != business.BusinessId) return BadRequest();
        var updatedBusiness = await _businessService.UpdateBusinessAsync(business);
        return Ok(updatedBusiness);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteBusiness(int id)
    {
        var success = await _businessService.DeleteBusinessAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}