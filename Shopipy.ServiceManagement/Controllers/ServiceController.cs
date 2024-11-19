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
[Route("[controller]")]
public class ServiceController : ControllerBase
{
    private readonly ServiceManagementService _serviceManagementService;
    private readonly CategoryService _categoryService;
    private readonly IMapper _mapper;

    public ServiceController(ServiceManagementService serviceManagementService, CategoryService categoryService, IMapper mapper)
    {
     _serviceManagementService = serviceManagementService;
    _categoryService = categoryService;
    _mapper = mapper;
    }
    [HttpGet("{businessId}/services")]
    public async Task<IActionResult> GetServices(int businessId)
    {
        var services = await _serviceManagementService.GetAllServicesInBusiness(businessId);
        var responseDtos = _mapper.Map<IEnumerable<ServiceResponseDto>>(services);
    
        return Ok(responseDtos);
    }

    [HttpGet("{businessId}/services/{id}")]
    public async Task<IActionResult> GetService(int businessId, int id)
    {
        var service = await _serviceManagementService.GetServiceByIdInBusiness(businessId, id);
        if (service == null) return NotFound();
        
        var responseDto = _mapper.Map<ServiceResponseDto>(service);

        return Ok(responseDto);
    }

    [HttpGet("{businessId}/services/category/{categoryId}")]
    public async Task<IActionResult> GetServicesByCategory(int businessId, int categoryId)
    {
        var category = await _categoryService.GetCategoryByIdAsync(categoryId);
        if (category == null)
        {
            return NotFound("Category does not exist");
        }
        var services = await _serviceManagementService.GetAllServicesByCategory(businessId, categoryId);
        var responseDtos = _mapper.Map<IEnumerable<ServiceResponseDto>>(services);
        
        return Ok(responseDtos);
    }

    [HttpPost("{businessId}")]
    public async Task<IActionResult> CreateService(int businessId, ServiceRequestDto request)
    {
        var categoryExists = await _categoryService.GetCategoryByIdAsync(request.CategoryId);
        if (categoryExists == null)
        {
            return NotFound("Category does not exist");
        }

        var service = _mapper.Map<Service>(request);
        service.BusinessId = businessId; // Associate with the business

        var createdService = await _serviceManagementService.CreateService(service);
        var responseDto = _mapper.Map<ServiceResponseDto>(createdService);

        return CreatedAtAction(nameof(GetService), new { businessId = service.BusinessId, id = createdService.ServiceId }, responseDto);
    }

    [HttpPut("{businessId}/services/{id}")]
    public async Task<IActionResult> UpdateService(int businessId, int id, ServiceRequestDto request)
    {
        var existingService = await _serviceManagementService.GetServiceByIdInBusiness(businessId, id);
        if (existingService == null) return NotFound();

        _mapper.Map(request, existingService);
        existingService.UpdatedAt = DateTime.UtcNow;

        var updatedService = await _serviceManagementService.UpdateService(existingService);
        var responseDto = _mapper.Map<ServiceResponseDto>(updatedService);

        return Ok(responseDto);
    }

    [HttpDelete("{businessId}/services/{id}")]
    public async Task<IActionResult> DeleteService(int businessId, int id)
    {
        var existingService = await _serviceManagementService.GetServiceByIdInBusiness(businessId, id);
        if (existingService == null) return NotFound();

        var success = await _serviceManagementService.DeleteService(id);
        if (!success) return BadRequest("Failed to delete the service.");

        return NoContent();
    }
}