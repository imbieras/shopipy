using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shopipy.Persistence.Models;
using Shopipy.ServiceManagement.DTOs;
using Shopipy.ServiceManagement.Mappings;
using Shopipy.ServiceManagement.Services;

namespace Shopipy.ServiceManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class ServiceController(ServiceManagementService serviceManagementService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetServices()
    {
        var services = await serviceManagementService.GetAllServices();
        var responseDtos = mapper.Map<IEnumerable<ServiceResponseDto>>(services);

        return Ok(responseDtos);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetService(int id)
    {
        var service = await serviceManagementService.GetServiceById(id);
        if (service == null) return NotFound();
        
        var responseDto = mapper.Map<ServiceResponseDto>(service);

        return Ok(responseDto);
    }

    [HttpPost]
    public async Task<IActionResult> CreateService(ServiceRequestDto request)
    {
        var service = mapper.Map<Service>(request);
        var createdService = await serviceManagementService.CreateService(service);
        var responseDto = mapper.Map<ServiceResponseDto>(createdService);

        return CreatedAtAction(nameof(GetService), new { id = createdService.ServiceId }, responseDto);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateService(int id, ServiceRequestDto request)
    {
        var existingService = await serviceManagementService.GetServiceById(id);
        if (existingService == null) return NotFound();

        mapper.Map(request, existingService);
        existingService.UpdatedAt = DateTime.UtcNow;
        
        var updatedService = await serviceManagementService.UpdateService(existingService);
        var responseDto = mapper.Map<ServiceResponseDto>(updatedService);

        return Ok(updatedService);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteService(int id)
    {
        var success = await serviceManagementService.DeleteService(id);
        if (!success) return NotFound();
        return NoContent();
    }
}