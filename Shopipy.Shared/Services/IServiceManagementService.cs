using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IServiceManagementService
{
    Task<Service> GetServiceByIdInBusiness(int businessId, int id);
    Task<IEnumerable<Service>> GetAllServicesByCategory(int businessId, int categoryId);
    Task<IEnumerable<Service>> GetAllServicesInBusiness(int businessId);
    Task<Service> CreateService(Service service);
    Task<Service> UpdateService(Service service);
    Task<bool> DeleteService(int id);
}