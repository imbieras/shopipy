using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.ServiceManagement.Services;

public class ServiceManagementService(IGenericRepository<Service> serviceRepository) : IServiceManagementService
{

    public async Task<Service?> GetServiceByIdInBusiness(int businessId, int id)
    {
        return await serviceRepository.GetByConditionAsync(s => s.BusinessId == businessId && s.ServiceId == id);
    }

    public async Task<IEnumerable<Service>> GetAllServicesByCategory(int businessId, int categoryId)
    {
        return await serviceRepository.GetAllByConditionAsync(cs => cs.BusinessId == businessId && cs.CategoryId == categoryId);
    }

    public async Task<IEnumerable<Service>> GetAllServicesInBusiness(int businessId)
    {
        return await serviceRepository.GetAllByConditionAsync(cs => cs.BusinessId == businessId);
    }

    public async Task<Service> CreateService(Service service)
    {
        return await serviceRepository.AddAsync(service);
    }

    public async Task<Service> UpdateService(Service service)
    {
        service.UpdatedAt = DateTime.Now;
        return await serviceRepository.UpdateAsync(service);
    }

    public async Task<bool> DeleteService(int id)
    {
        return await serviceRepository.DeleteAsync(id);
    }
}