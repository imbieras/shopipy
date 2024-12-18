using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IDiscountService
{
    Task<IEnumerable<Discount>> GetAllDiscountsAsync();
    Task<IEnumerable<Discount>> GetAllDiscountsInBusinessAsync(int businessId);
    Task<Discount?> GetDiscountByIdAsync(int id);
    Task<Discount?> GetDiscountByIdInBusinessAsync(int businessId, int id);
    Task<Discount> CreateDiscountAsync(Discount discount);
    Task<Discount> UpdateDiscountAsync(Discount discount, DateTime? effectiveTo);
    Task<bool> DeleteDiscountAsync(int id);
}