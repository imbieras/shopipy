using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.GiftcardManagement.Services
{
    public class GiftCardService(IGenericRepository<GiftCard> _giftCardRepository) : IGiftCardService
    {
        public async Task<GiftCard> CreateGiftCardAsync(GiftCard giftCard, int businessId)
        {
            giftCard.BusinessId = businessId;
            giftCard.GiftCardCode = Guid.NewGuid().ToString("N").Substring(0, 12).ToUpper();

            var createdGiftCard = await _giftCardRepository.AddAsync(giftCard);
            return createdGiftCard;
        }

        public async Task<IEnumerable<GiftCard>> GetAllGiftCardsOfBusinessAsync(int businessId, int? top = null, int? skip = 0)
        {
            if (skip.HasValue || top.HasValue)
            {
                return await _giftCardRepository.GetAllByConditionWithPaginationAsync(
                    g => g.BusinessId == businessId,
                    skip ?? 0,
                    top ?? int.MaxValue
                );
            }

            return await _giftCardRepository.GetAllByConditionAsync(g => g.BusinessId == businessId);
        }

        public async Task<int> GetGiftCardCountAsync(int businessId)
        {
            return await _giftCardRepository.GetCountByConditionAsync(g => g.BusinessId == businessId);
        }

        public async Task<GiftCard?> GetGiftCardByIdAsync(int giftCardId, int businessId)
        {
            var giftCard = await _giftCardRepository.GetByConditionAsync(g => g.GiftCardId == giftCardId && g.BusinessId == businessId);
            return giftCard;
        }

        public async Task<GiftCard> UpdateGiftCardAsync(GiftCard giftCard)
        {
            giftCard.UpdatedAt = DateTime.UtcNow;
            return await _giftCardRepository.UpdateAsync(giftCard);
        }

        public async Task<GiftCard> UpdateGiftCardLeftAmountAsync(int giftCardId, int businessId, decimal amountPaid)
        {
            var giftCard = await _giftCardRepository.GetByConditionAsync(g => g.GiftCardId == giftCardId && g.BusinessId == businessId);

            giftCard.UpdatedAt = DateTime.UtcNow;
            giftCard.AmountLeft = giftCard.AmountLeft - amountPaid;

            return await _giftCardRepository.UpdateAsync(giftCard);
        }

        public async Task<bool> DeleteGiftCardAsync(int giftCardId, int businessId)
        {
            var giftCard = await _giftCardRepository.GetByConditionAsync(g => g.GiftCardId == giftCardId && g.BusinessId == businessId);
            if (giftCard == null) return false;

            return await _giftCardRepository.DeleteAsync(giftCard.GiftCardId);
        }
    }

}
