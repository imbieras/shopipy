using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IBusinessService
{
    Task<Business?> GetBusinessByIdAsync(int id);
}