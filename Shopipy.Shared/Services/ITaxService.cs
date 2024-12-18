using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface ITaxService
{
    Task<IEnumerable<TaxRate>> GetAllTaxRatesByBusinessAsync(int businessId);
    Task<int> GetTaxRateCountAsync(int businessId);
    Task<TaxRate?> GetTaxRateByIdAndBusinessAsync(int taxRateId, int businessId);
    Task<TaxRate> CreateTaxRateAsync(TaxRate taxRate);
    Task<TaxRate> UpdateTaxRateAsync(TaxRate taxRate, DateTime? effectiveTo);
    Task<bool> DeleteTaxRateAsync(int taxRateId);
}