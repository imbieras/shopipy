using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.DiscountManagement.Services;

public class DiscountService(IGenericRepository<Discount> discountRepository) : IDiscountService
{
    public async Task<IEnumerable<Discount>> GetAllDiscountsAsync()
    {
        return await discountRepository.GetAllAsync();
    }

    public async Task<IEnumerable<Discount>> GetAllDiscountsInBusinessAsync(int businessId)
    {
        return await discountRepository.GetAllByConditionAsync(d => d.BusinessId == businessId);
    }

    public async Task<Discount> GetDiscountByIdAsync(int id)
    {
        return await discountRepository.GetByIdAsync(id);
    }

    public async Task<Discount?> GetDiscountByIdInBusinessAsync(int businessId, int id)
    {
        return await discountRepository.GetByConditionAsync(d => d.BusinessId == businessId && d.DiscountId == id);
    }

    public async Task<Discount> CreateDiscountAsync(Discount discount)
    {
        return await discountRepository.AddAsync(discount);
    }

    public async Task<Discount> UpdateDiscountAsync(Discount discount, DateTime? effectiveTo)
    {
        discount.EffectiveTo = effectiveTo;

        return await discountRepository.UpdateAsync(discount);
    }

    public async Task<bool> DeleteDiscountAsync(int id)
    {
        var discount = await discountRepository.GetByIdAsync(id);

        discount.EffectiveTo = DateTime.UtcNow;

        await discountRepository.UpdateAsync(discount);
        return true;
    }
}