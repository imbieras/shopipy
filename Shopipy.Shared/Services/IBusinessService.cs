using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IBusinessService
{
    Task<Business?> GetBusinessByIdAsync(int id);
    Task<IEnumerable<Business>> GetAllBusinessesAsync();
    Task<Business> CreateBusinessAsync(Business business);
    Task<Business> UpdateBusinessAsync(Business business);
    Task<bool> DeleteBusinessAsync(int id);
}