using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.BusinessManagement.Services;

public class BusinessService(IGenericRepository<Business> businessRepository) : IBusinessService
{

    public async Task<IEnumerable<Business>> GetAllBusinessesAsync()
    {
        return await businessRepository.GetAllAsync();
    }

    public async Task<Business?> GetBusinessByIdAsync(int id)
    {
        return await businessRepository.GetByIdAsync(id);
    }

    public async Task<Business> CreateBusinessAsync(Business business)
    {
        return await businessRepository.AddAsync(business);
    }

    public async Task<Business> UpdateBusinessAsync(Business business)
    {
        return await businessRepository.UpdateAsync(business);
    }

    public async Task<bool> DeleteBusinessAsync(int id)
    {
        return await businessRepository.DeleteAsync(id);
    }
}