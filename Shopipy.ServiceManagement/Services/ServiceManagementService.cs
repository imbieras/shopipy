using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;

namespace Shopipy.ServiceManagement.Services;

public class ServiceManagementService
{
    private readonly IGenericRepository<Service> _serviceRepository;

    public ServiceManagementService(IGenericRepository<Service> serviceRepository)
    {
        _serviceRepository = serviceRepository;
    }
    
    public async Task<Service> GetServiceByIdInBusiness(int businessId ,int id)
    {
        return await _serviceRepository.GetByConditionAsync(s => s.BusinessId == businessId && s.ServiceId == id);
    }
    
    public async Task<IEnumerable<Service>> GetAllServicesByCategory(int businessId, int categoryId)
    {
        return await _serviceRepository.GetAllByConditionAsync(cs => cs.BusinessId == businessId && cs.CategoryId == categoryId);
    }

    public async Task<IEnumerable<Service>> GetAllServicesInBusiness(int businessId)
    {
        return await _serviceRepository.GetAllByConditionAsync(cs => cs.BusinessId == businessId);
    }

    public async Task<Service> CreateService(Service service)
    {
        return await _serviceRepository.AddAsync(service);
    }

    public async Task<Service> UpdateService(Service service)
    {
        return await _serviceRepository.UpdateAsync(service);
    }

    public async Task<bool> DeleteService(int id)
    {
        return await _serviceRepository.DeleteAsync(id);
    }
}