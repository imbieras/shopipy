using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;

namespace Shopipy.GiftcardManagement.Services
{
    public class GiftCardService(IGenericRepository<GiftCard> giftCardRepository) : IGiftCardService
    {
        public async Task<GiftCard> CreateGiftCardAsync(GiftCard giftCard, int businessId)
        {
            giftCard.BusinessId = businessId;
            giftCard.GiftCardCode = Guid.NewGuid().ToString("N")[..12].ToUpper();

            if (giftCard.AmountOriginal < 0)
            {
                throw new ArgumentException("The amount has to be positive.");
            }

            if (giftCard.ValidFrom > giftCard.ValidUntil)
            {
                throw new ArgumentException("The expiration date must be later than the start date.");
            }

            var createdGiftCard = await giftCardRepository.AddAsync(giftCard);
            return createdGiftCard;
        }

        public async Task<IEnumerable<GiftCard>> GetAllGiftCardsOfBusinessAsync(int businessId, int? top = null, int? skip = 0)
        {
            if (skip.HasValue || top.HasValue)
            {
                return await giftCardRepository.GetAllByConditionWithPaginationAsync(
                g => g.BusinessId == businessId,
                skip ?? 0,
                top ?? int.MaxValue
                );
            }

            return await giftCardRepository.GetAllByConditionAsync(g => g.BusinessId == businessId);
        }

        public async Task<int> GetGiftCardCountAsync(int businessId)
        {
            return await giftCardRepository.GetCountByConditionAsync(g => g.BusinessId == businessId);
        }

        public async Task<GiftCard?> GetGiftCardByIdAsync(int id)
        {
            return await giftCardRepository.GetByIdAsync(id);
        }

        public async Task<GiftCard?> GetGiftCardByIdInBusinessAsync(int giftCardId, int businessId)
        {
            return await giftCardRepository.GetByConditionAsync(g => g.GiftCardId == giftCardId && g.BusinessId == businessId);
        }

        public async Task<GiftCard> UpdateGiftCardAsync(GiftCard giftCard)
        {
            giftCard.UpdatedAt = DateTime.UtcNow;
            return await giftCardRepository.UpdateAsync(giftCard);
        }

        public async Task<GiftCard> UpdateGiftCardLeftAmountAsync(int giftCardId, int businessId, decimal amountPaid)
        {
            var giftCard = await giftCardRepository.GetByConditionAsync(g => g.GiftCardId == giftCardId && g.BusinessId == businessId);

            if (giftCard == null)
            {
                throw new ArgumentException("Gift card not found.");
            }

            giftCard.UpdatedAt = DateTime.UtcNow;
            giftCard.AmountLeft -= amountPaid;

            return await giftCardRepository.UpdateAsync(giftCard);
        }


        public async Task<GiftCard?> GetGiftCardByHashAsync(int businessId, string hash)
        {
            if (string.IsNullOrWhiteSpace(hash))
            {
                throw new ArgumentException("Hash is required", nameof(hash));
            }

            var giftCard = await giftCardRepository.GetByConditionAsync(g => g.GiftCardCode == hash && g.BusinessId == businessId);

            return giftCard;
        }

        public async Task<bool> DeleteGiftCardAsync(int giftCardId, int businessId)
        {
            var giftCard = await giftCardRepository.GetByConditionAsync(g => g.GiftCardId == giftCardId && g.BusinessId == businessId);
            if (giftCard == null) return false;

            return await giftCardRepository.DeleteAsync(giftCard.GiftCardId);
        }
    }

}