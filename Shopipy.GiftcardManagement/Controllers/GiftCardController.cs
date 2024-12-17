using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.Persistence.Models;
using Shopipy.GiftCardManagement.DTOs;
using Shopipy.Shared;
using Shopipy.Shared.DTOs;
using Shopipy.Shared.Services;

namespace Shopipy.GiftCardManagement.Controllers;

[Route("businesses/{businessId:int}/giftcards")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class GiftCardController(IGiftCardService giftCardService, IBusinessService businessService, IMapper mapper, ILogger<GiftCardController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGiftCard(GiftCardRequestDto dto, int businessId)
    {
        var business = await businessService.GetBusinessByIdAsync(businessId);
        if (business == null)
        {
            logger.LogWarning("Business with ID {BusinessId} not found for gift card creation.", businessId);
            return NotFound();
        }
        
        var giftCard = mapper.Map<GiftCard>(dto);

        var createdGiftCard = await giftCardService.CreateGiftCardAsync(giftCard, businessId);

        var giftCardResponseDto = mapper.Map<GiftCardResponseDto>(createdGiftCard);

        return CreatedAtAction(nameof(GetGiftCardById), new { businessId, giftCardId = createdGiftCard.GiftCardId }, giftCardResponseDto);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> GetAllGiftCards(int businessId, int? top = null, int? skip = null)
    {
        var giftCards = await giftCardService.GetAllGiftCardsOfBusinessAsync(businessId, top, skip);
        var count = await giftCardService.GetGiftCardCountAsync(businessId);

        return Ok(new PaginationResultDto<GiftCardResponseDto> { Data = giftCards.Select(mapper.Map<GiftCardResponseDto>), Count = count });
    }

    [HttpGet("{giftCardId:int}")]
    public async Task<ActionResult<GiftCardResponseDto>> GetGiftCardById(int businessId, int giftCardId)
    {
        var giftCard = await giftCardService.GetGiftCardByIdAsync(giftCardId, businessId);
        if (giftCard == null)
        {
            logger.LogWarning("Gift card with ID {GiftCardId} not found for business {BusinessId}.", giftCardId, businessId);
            return NotFound();
        }

        var giftCardResponseDto = mapper.Map<GiftCardResponseDto>(giftCard);
        return Ok(giftCardResponseDto);
    }

    [HttpPut("{giftCardId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<GiftCardResponseDto>> UpdateGiftCard(int giftCardId, GiftCardRequestDto dto, int businessId)
    {
        var existingGiftCard = await giftCardService.GetGiftCardByIdAsync(giftCardId, businessId);

        if (existingGiftCard == null)
        {
            logger.LogWarning("Gift card with ID {GiftCardId} not found for update in business {BusinessId}.", giftCardId, businessId);
            return NotFound();
        }

        mapper.Map(dto, existingGiftCard);

        var updatedGiftCard = await giftCardService.UpdateGiftCardAsync(existingGiftCard);
        var giftCardResponseDto = mapper.Map<GiftCardResponseDto>(updatedGiftCard);

        return Ok(giftCardResponseDto);
    }

    [HttpDelete("{giftCardId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteGiftCard(int businessId, int giftCardId)
    {
        var success = await giftCardService.DeleteGiftCardAsync(giftCardId, businessId);
        if (success)
        {
            return NoContent();
        }

        logger.LogWarning("Failed to delete gift card with ID {GiftCardId} for business {BusinessId}.", giftCardId, businessId);
        return NotFound();

    }
}