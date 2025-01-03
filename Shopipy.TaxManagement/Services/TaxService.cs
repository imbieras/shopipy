using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.TaxManagement.Services
{
    public class TaxService(IGenericRepository<TaxRate> taxRateRepository) : ITaxService
    {
        public async Task<IEnumerable<TaxRate>> GetAllTaxRatesAsync()
        {
            return await taxRateRepository.GetAllAsync();
        }

        public async Task<TaxRate?> GetTaxRateByIdAsync(int id)
        {
            return await taxRateRepository.GetByIdAsync(id);
        }

        public async Task<IEnumerable<TaxRate>> GetAllTaxRatesByBusinessAsync(int businessId)
        {
            return await taxRateRepository.GetAllByConditionAsync(tr => tr.BusinessId == businessId);
        }

        public async Task<int> GetTaxRateCountAsync(int businessId)
        {
            return await taxRateRepository.GetCountByConditionAsync(tr => tr.BusinessId == businessId);
        }

        public async Task<TaxRate?> GetTaxRateByIdAndBusinessAsync(int taxRateId, int businessId)
        {
            return await taxRateRepository.GetByConditionAsync(tr => tr.TaxRateId == taxRateId && tr.BusinessId == businessId);
        }

        public async Task<TaxRate> CreateTaxRateAsync(TaxRate taxRate)
        {
            taxRate.CreatedAt = DateTime.UtcNow;
            return await taxRateRepository.AddAsync(taxRate);
        }

        public async Task<TaxRate> UpdateTaxRateAsync(TaxRate taxRate, DateTime? effectiveTo)
        {
            taxRate.EffectiveTo = effectiveTo;
            return await taxRateRepository.UpdateAsync(taxRate);
        }

        public async Task<bool> DeleteTaxRateAsync(int id)
        {
            var taxRate = await taxRateRepository.GetByIdAsync(id);
            if (taxRate == null)
            {
                return false;
            }

            taxRate.EffectiveTo = DateTime.UtcNow;
            await taxRateRepository.UpdateAsync(taxRate);

            return true;
        }
    }
}