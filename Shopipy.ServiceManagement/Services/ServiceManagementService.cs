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

    public async Task<IEnumerable<Service>> GetAllServices()
    {
        return await _serviceRepository.GetAllAsync();
    }

    public async Task<Service> GetServiceById(int id)
    {
        return await _serviceRepository.GetByIdAsync(id);
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