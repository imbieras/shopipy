using System.Collections;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shopipy.CategoryManagement.Services;
using Shopipy.Persistence.Models;
using Shopipy.ServiceManagement.DTOs;
using Shopipy.ServiceManagement.Mappings;
using Shopipy.ServiceManagement.Services;

namespace Shopipy.ServiceManagement.Controllers;

[ApiController]
[Route("[controller]/{businessId}")]
public class ServiceController(
    ServiceManagementService serviceManagementService, 
    CategoryService categoryService, 
    IMapper mapper
) : ControllerBase
{
    
    [HttpGet("/services")]
    public async Task<IActionResult> GetServices(int businessId, [FromQuery] int? categoryId = null)
    {
        if (categoryId.HasValue)
        {
            var category = await categoryService.GetCategoryByIdAsync(categoryId.Value);
            if (category == null) return NotFound("Category does not exist");
            
            var services = await serviceManagementService.GetAllServicesByCategory(businessId, categoryId.Value);
            return Ok(mapper.Map<IEnumerable<ServiceResponseDto>>(services));
        }

        var allServices = await serviceManagementService.GetAllServicesInBusiness(businessId);
        return Ok(mapper.Map<IEnumerable<ServiceResponseDto>>(allServices));
    }

    [HttpGet("/services/{id}")]
    public async Task<IActionResult> GetService(int businessId, int id)
    {
        var service = await serviceManagementService.GetServiceByIdInBusiness(businessId, id);
        if (service == null) return NotFound();
        
        var responseDto = mapper.Map<ServiceResponseDto>(service);

        return Ok(responseDto);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreateService(int businessId, ServiceRequestDto request)
    {
        var categoryExists = await categoryService.GetCategoryByIdAsync(request.CategoryId);
        if (categoryExists == null)
        {
            return NotFound("Category does not exist");
        }

        var service = mapper.Map<Service>(request);
        service.BusinessId = businessId; // Associate with the business

        var createdService = await serviceManagementService.CreateService(service);
        var responseDto = mapper.Map<ServiceResponseDto>(createdService);

        return CreatedAtAction(nameof(GetService), new { businessId = service.BusinessId, id = createdService.ServiceId }, responseDto);
    }

    [HttpPut("/services/{id}")]
    public async Task<IActionResult> UpdateService(int businessId, int id, ServiceRequestDto request)
    {
        var existingService = await serviceManagementService.GetServiceByIdInBusiness(businessId, id);
        if (existingService == null) return NotFound();

        mapper.Map(request, existingService);
        existingService.UpdatedAt = DateTime.UtcNow;

        var updatedService = await serviceManagementService.UpdateService(existingService);
        var responseDto = mapper.Map<ServiceResponseDto>(updatedService);

        return Ok(responseDto);
    }

    [HttpDelete("/services/{id}")]
    public async Task<IActionResult> DeleteService(int businessId, int id)
    {
        var existingService = await serviceManagementService.GetServiceByIdInBusiness(businessId, id);
        if (existingService == null) return NotFound();

        var success = await serviceManagementService.DeleteService(id);
        if (!success) return BadRequest("Failed to delete the service.");

        return NoContent();
    }
}