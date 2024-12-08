using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface ITaxService
{
    Task<IEnumerable<TaxRate>> GetAllTaxRatesByBusinessAsync(int businessId);
    Task<TaxRate?> GetTaxRateByIdAndBusinessAsync(int taxRateId, int businessId);
    Task<TaxRate> AddTaxRateAsync(TaxRate taxRate);
    Task<TaxRate> UpdateTaxRateAsync(TaxRate taxRate);
    Task<bool> DeleteTaxRateAsync(int taxRateId);
    Task<IEnumerable<TaxRate>> GetAllTaxRatesAsync();
    Task<TaxRate?> GetTaxRateByIdAsync(int taxRateId);
}
