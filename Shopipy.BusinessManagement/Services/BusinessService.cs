using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.BusinessManagement.Services;

public class BusinessService : IBusinessService
{
    private readonly IGenericRepository<Business> _businessRepository;
    public BusinessService(IGenericRepository<Business> businessRepository)
    {
        _businessRepository = businessRepository;
    }

    public async Task<IEnumerable<Business>> GetAllBusinessesAsync()
    {
        return await _businessRepository.GetAllAsync();
    }

    public async Task<Business?> GetBusinessByIdAsync(int id)
    {
        return await _businessRepository.GetByIdAsync(id);
    }

    public async Task<Business> CreateBusinessAsync(Business business)
    {
        return await _businessRepository.AddAsync(business);
    }

    public async Task<Business> UpdateBusinessAsync(Business business)
    {
        return await _businessRepository.UpdateAsync(business);
    }

    public async Task<bool> DeleteBusinessAsync(int id)
    {
        return await _businessRepository.DeleteAsync(id);
    }
}