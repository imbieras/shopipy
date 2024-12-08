using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Shopipy.Shared.Services;

namespace Shopipy.TaxManagement.Services
{
    public class TaxService(IGenericRepository<TaxRate> taxRateRepository) : ITaxService
    {
        // Get all tax rates
        public async Task<IEnumerable<TaxRate>> GetAllTaxRatesAsync()
        {
            return await taxRateRepository.GetAllAsync();
        }

        // Get tax rate by ID
        public async Task<TaxRate> GetTaxRateByIdAsync(int id)
        {
            return await taxRateRepository.GetByIdAsync(id);
        }

        // Add a new tax rate
        public async Task<IEnumerable<TaxRate>> GetAllTaxRatesByBusinessAsync(int businessId)
        {
            return await taxRateRepository.GetAllByConditionAsync(tr => tr.BusinessId == businessId);
        }

        public async Task<TaxRate?> GetTaxRateByIdAndBusinessAsync(int taxRateId, int businessId)
        {
            return await taxRateRepository.GetByConditionAsync(tr => tr.TaxRateId == taxRateId && tr.BusinessId == businessId);
        }

        public async Task<TaxRate> AddTaxRateAsync(TaxRate taxRate)
        {
            taxRate.CreatedAt = DateTime.UtcNow;
            return await taxRateRepository.AddAsync(taxRate);
        }

        // Update an existing tax rate
        public async Task<TaxRate> UpdateTaxRateAsync(TaxRate taxRate)
        {
            return await taxRateRepository.UpdateAsync(taxRate);
        }

        // Delete a tax rate
        public async Task<bool> DeleteTaxRateAsync(int id)
        {
            return await taxRateRepository.DeleteAsync(id);
        }
    }
}
