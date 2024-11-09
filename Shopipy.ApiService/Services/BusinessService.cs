using Shopipy.ApiService.Models;
using Shopipy.ApiService.Repositories;

namespace Shopipy.ApiService.Services;

public class BusinessService
{
    private readonly IBusinessRepository _businessRepository;

    public BusinessService(IBusinessRepository businessRepository)
    {
        _businessRepository = businessRepository;
    }

    public async Task<IEnumerable<Business>> GetAllBusinessesAsync()
    {
        return await _businessRepository.GetAllBusinessesAsync();
    }

    public async Task<Business> GetBusinessByIdAsync(int id)
    {
        return await _businessRepository.GetBusinessByIdAsync(id);
    }

    public async Task<Business> CreateBusinessAsync(Business business)
    {
        return await _businessRepository.AddBusinessAsync(business);
    }

    public async Task<Business> UpdateBusinessAsync(Business business)
    {
        return await _businessRepository.UpdateBusinessAsync(business);
    }

    public async Task<bool> DeleteBusinessAsync(int id)
    {
        return await _businessRepository.DeleteBusinessAsync(id);
    }
}