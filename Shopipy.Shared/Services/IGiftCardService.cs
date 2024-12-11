using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.Shared.Services
{
    public interface IGiftCardService
    {
        Task<GiftCard> CreateGiftCardAsync(GiftCard giftCard, int businessId);
        Task<IEnumerable<GiftCard>> GetAllGiftCardsOfBusinessAsync(int businessId, int? top = null, int? skip = 0);
        Task<int> GetGiftCardCountAsync(int businessId);
        Task<GiftCard?> GetGiftCardByIdAsync(int giftCardId, int businessId);
        Task<GiftCard> UpdateGiftCardAsync(GiftCard giftCard);
        Task<bool> DeleteGiftCardAsync(int giftCardId, int businessId);

    }
}
