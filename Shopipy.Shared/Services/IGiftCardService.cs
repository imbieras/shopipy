using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public interface IGiftCardService
{
    Task<GiftCard> CreateGiftCardAsync(GiftCard giftCard, int businessId);
    Task<IEnumerable<GiftCard>> GetAllGiftCardsOfBusinessAsync(int businessId, int? top = null, int? skip = 0);
    Task<int> GetGiftCardCountAsync(int businessId);
    Task<GiftCard?> GetGiftCardByIdAsync(int id);
    Task<GiftCard?> GetGiftCardByIdInBusinessAsync(int giftCardId, int businessId);
    Task<GiftCard> UpdateGiftCardAsync(GiftCard giftCard);
    Task<bool> DeleteGiftCardAsync(int giftCardId, int businessId);
    Task<GiftCard> GetGiftCardByHashAsync(int businessId, string? giftCardHash);
    Task<GiftCard> UpdateGiftCardLeftAmountAsync(int businessId, int giftCardId, decimal amountPaid);
}