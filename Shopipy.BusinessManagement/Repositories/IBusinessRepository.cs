using Shopipy.Persistence.Models;

namespace Shopipy.BusinessManagement.Repositories;

//mb we will have 2 implementations of a repository: database-backed and cache-backed
public interface IBusinessRepository
{
    Task<IEnumerable<Business>> GetAllBusinessesAsync();
    Task<Business> GetBusinessByIdAsync(int id);
    Task<Business> AddBusinessAsync(Business business);
    Task<Business> UpdateBusinessAsync(Business business);
    Task<bool> DeleteBusinessAsync(int id);
}