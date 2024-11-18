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
    [HttpGet]
    public async Task<IActionResult> GetServices()
    {
        var services = await _serviceManagementService.GetAllServices();
        var responseDtos = _mapper.Map<IEnumerable<ServiceResponseDto>>(services);

        return Ok(responseDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetService(int id)
    {
        var service = await _serviceManagementService.GetServiceById(id);
        if (service == null) return NotFound();
        
        var responseDto = _mapper.Map<ServiceResponseDto>(service);

        return Ok(responseDto);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<IActionResult> GetServicesByCategory(int categoryId)
    {
        var category = await _categoryService.GetCategoryByIdAsync(categoryId);
        if (category == null)
        {
            return NotFound("Category does not exist");
        }
        var services = await _serviceManagementService.GetAllServicesByCategory(categoryId);
        var responseDtos = _mapper.Map<IEnumerable<ServiceResponseDto>>(services);
        
        return Ok(responseDtos);
    }

    [HttpPost]
    public async Task<IActionResult> CreateService(ServiceRequestDto request)
    {
        var categoryExists = _categoryService.GetCategoryByIdAsync(request.CategoryId);

        if (categoryExists == null)
        {
            return NotFound("Category does not exist");
        }
        
        var service = _mapper.Map<Service>(request);
        var createdService = await _serviceManagementService.CreateService(service);
        var responseDto = _mapper.Map<ServiceResponseDto>(createdService);

        return CreatedAtAction(nameof(GetService), new { id = createdService.ServiceId }, responseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(int id, ServiceRequestDto request)
    {
        var existingService = await _serviceManagementService.GetServiceById(id);
        if (existingService == null) return NotFound();

        _mapper.Map(request, existingService);
        existingService.UpdatedAt = DateTime.UtcNow;
        
        var updatedService = await _serviceManagementService.UpdateService(existingService);
        var responseDto = _mapper.Map<ServiceResponseDto>(updatedService);

        return Ok(updatedService);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var success = await _serviceManagementService.DeleteService(id);
        if (!success) return NotFound();
        return NoContent();
    }
}