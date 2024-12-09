using Shopipy.DiscountManagement.Services;
using Shopipy.Shared.Services;

namespace Shopipy.DiscountManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddDiscountManagement(this IServiceCollection discounts)
    {
        discounts.AddScoped<IDiscountService, DiscountService>();
        return discounts;
    }
}