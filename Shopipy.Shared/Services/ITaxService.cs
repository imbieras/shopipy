using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface ITaxService
{
    Task<IEnumerable<TaxRate>> GetAllTaxRatesByBusinessAsync(int businessId, int? top = null, int? skip = null);
    Task<int> GetTaxRateCountAsync(int businessId);
    Task<TaxRate?> GetTaxRateByIdAndBusinessAsync(int taxRateId, int businessId);
    Task<TaxRate> AddTaxRateAsync(TaxRate taxRate);
    Task<TaxRate> UpdateTaxRateAsync(TaxRate taxRate);
    Task<bool> DeleteTaxRateAsync(int taxRateId);
}
