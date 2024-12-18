using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.Persistence.Models;
using Shopipy.ServiceManagement.DTOs;
using Shopipy.Shared;
using Shopipy.Shared.Services;

namespace Shopipy.ServiceManagement.Controllers;

[Authorize]
[ApiController]
[Route("businesses/{businessId:int}/services")]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class ServiceController(IServiceManagementService serviceManagementService, IBusinessService businessService, ICategoryService categoryService, IMapper mapper, ILogger<ServiceController> logger) : ControllerBase
{

    [HttpGet]
    public async Task<IActionResult> GetServices(int businessId, [FromQuery] int? categoryId = null)
    {
        if (categoryId.HasValue)
        {
            var category = await categoryService.GetCategoryByIdAsync(categoryId.Value);
            if (category == null)
            {
                logger.LogWarning("Category ID {CategoryId} does not exist in business {BusinessId}.", categoryId, businessId);
                return NotFound("Category does not exist");
            }

            var services = await serviceManagementService.GetAllServicesByCategory(businessId, categoryId.Value);
            return Ok(mapper.Map<IEnumerable<ServiceResponseDto>>(services));
        }

        var allServices = await serviceManagementService.GetAllServicesInBusiness(businessId);
        return Ok(mapper.Map<IEnumerable<ServiceResponseDto>>(allServices));
    }

    [HttpGet("{serviceId:int}")]
    public async Task<IActionResult> GetService(int businessId, int serviceId)
    {
        var service = await serviceManagementService.GetServiceByIdInBusiness(businessId, serviceId);
        if (service != null)
        {
            return Ok(mapper.Map<ServiceResponseDto>(service));
        }

        logger.LogWarning("Service ID {ServiceId} not found in Business ID {BusinessId}.", serviceId, businessId);
        return NotFound();
    }

    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> CreateService(int businessId, ServiceRequestDto request)
    {
        var business = await businessService.GetBusinessByIdAsync(businessId);
        if (business == null)
        {
            logger.LogWarning("Business with ID {BusinessId} not found for service creation.", businessId);
            return NotFound();
        }

        var categoryExists = await categoryService.GetCategoryByIdInBusinessAsync(businessId, request.CategoryId);
        if (categoryExists == null)
        {
            logger.LogWarning("Category ID {CategoryId} does not exist in Business ID {BusinessId}.", request.CategoryId, businessId);
            return NotFound("Category does not exist in business");
        }

        var service = mapper.Map<Service>(request);
        service.BusinessId = businessId;

        var createdService = await serviceManagementService.CreateService(service);

        return CreatedAtAction(nameof(GetService), new { businessId = service.BusinessId, serviceId = createdService.ServiceId }, mapper.Map<ServiceResponseDto>(createdService));
    }

    [HttpPut("{serviceId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> UpdateService(int businessId, int serviceId, ServiceRequestDto request)
    {
        var existingService = await serviceManagementService.GetServiceByIdInBusiness(businessId, serviceId);
        if (existingService == null)
        {
            logger.LogWarning("Service ID {ServiceId} not found in Business ID {BusinessId}.", serviceId, businessId);
            return NotFound();
        }

        mapper.Map(request, existingService);
        existingService.UpdatedAt = DateTime.UtcNow;

        var updatedService = await serviceManagementService.UpdateService(existingService);

        return Ok(mapper.Map<ServiceResponseDto>(updatedService));
    }

    [HttpDelete("{serviceId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteService(int businessId, int serviceId)
    {
        var existingService = await serviceManagementService.GetServiceByIdInBusiness(businessId, serviceId);
        if (existingService == null)
        {
            logger.LogWarning("Service ID {ServiceId} not found in Business ID {BusinessId}.", serviceId, businessId);
            return NotFound();
        }

        var success = await serviceManagementService.DeleteService(serviceId);
        if (success)
        {
            return NoContent();
        }

        logger.LogError("Failed to delete Service ID {ServiceId} in Business ID {BusinessId}.", serviceId, businessId);
        return NotFound();
    }
}