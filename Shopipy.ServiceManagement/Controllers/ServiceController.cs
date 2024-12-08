using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.Persistence.Models;
using Shopipy.ServiceManagement.DTOs;
using Shopipy.ServiceManagement.Mappings;
using Shopipy.Shared.Services;

namespace Shopipy.ServiceManagement.Controllers;

[Authorize]
[ApiController]
[Route("businesses/{businessId}/services")]
public class ServiceController(
    IServiceManagementService serviceManagementService, 
    ICategoryService categoryService, 
    IMapper mapper
) : ControllerBase
{
    
    [HttpGet]
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

    [HttpGet("{serviceId}")]
    public async Task<IActionResult> GetService(int businessId, int serviceId)
    {
        var service = await serviceManagementService.GetServiceByIdInBusiness(businessId, serviceId);
        if (service == null) return NotFound();
        
        var responseDto = mapper.Map<ServiceResponseDto>(service);

        return Ok(responseDto);
    }

    [HttpPost]
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

    [HttpPut("{serviceId}")]
    public async Task<IActionResult> UpdateService(int businessId, int serviceId, ServiceRequestDto request)
    {
        var existingService = await serviceManagementService.GetServiceByIdInBusiness(businessId, serviceId);
        if (existingService == null) return NotFound();

        mapper.Map(request, existingService);
        existingService.UpdatedAt = DateTime.UtcNow;

        var updatedService = await serviceManagementService.UpdateService(existingService);
        var responseDto = mapper.Map<ServiceResponseDto>(updatedService);

        return Ok(responseDto);
    }

    [HttpDelete("{serviceId}")]
    public async Task<IActionResult> DeleteService(int businessId, int serviceId)
    {
        var existingService = await serviceManagementService.GetServiceByIdInBusiness(businessId, serviceId);
        if (existingService == null) return NotFound();

        var success = await serviceManagementService.DeleteService(serviceId);
        if (!success) return BadRequest("Failed to delete the service.");

        return NoContent();
    }
}