using Shopipy.GiftcardManagement.Services;
using Shopipy.Shared.Services;

namespace Shopipy.GiftcardManagement;

public static class DependencyInjection
{
    public static IServiceCollection AddGiftCardManagement(this IServiceCollection giftCards)
    {
        giftCards.AddScoped<IGiftCardService, GiftCardService>();
        return giftCards;
    }
}
