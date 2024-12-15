using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.Persistence.Models;
using Shopipy.GiftCardManagement.DTOs;
using Shopipy.Shared;
using Shopipy.Shared.DTOs;
using Shopipy.Shared.Services;

namespace Shopipy.GiftCardManagement.Controllers;

[Route("businesses/{businessId}/giftcards")]
[ApiController]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class GiftCardController(IGiftCardService _giftCardService, IMapper _mapper) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> CreateGiftCard(GiftCardRequestDTO dto, int businessId)
    {
        var giftCard = _mapper.Map<GiftCard>(dto);

        var createdGiftCard = await _giftCardService.CreateGiftCardAsync(giftCard, businessId);

        var giftCardResponseDTO = _mapper.Map<GiftCardResponseDTO>(createdGiftCard);

        return CreatedAtAction(nameof(GetGiftCardById), new { businessId, giftCardId = createdGiftCard.GiftCardId }, giftCardResponseDTO);
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> GetAllGiftCards(int businessId, int? top = null, int? skip = null)
    {
        var giftCards = await _giftCardService.GetAllGiftCardsOfBusinessAsync(businessId, top, skip);
        var count = await _giftCardService.GetGiftCardCountAsync(businessId);

        return Ok(new PaginationResultDto<GiftCardResponseDTO>
        {
            Data = giftCards.Select(_mapper.Map<GiftCardResponseDTO>),
            Count = count
        });
    }

    [HttpGet("{giftCardId}")]
    public async Task<ActionResult<GiftCardResponseDTO>> GetGiftCardById(int businessId, int giftCardId)
    {
        var giftCard = await _giftCardService.GetGiftCardByIdAsync(giftCardId, businessId);
        if (giftCard == null)
        {
            return NotFound();
        }

        var giftCardResponseDTO = _mapper.Map<GiftCardResponseDTO>(giftCard);
        return Ok(giftCardResponseDTO);
    }

    [HttpPut("{giftCardId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<GiftCardResponseDTO>> UpdateGiftCard(int giftCardId, GiftCardRequestDTO dto, int businessId)
    {
        var existingGiftCard = await _giftCardService.GetGiftCardByIdAsync(giftCardId, businessId);

        if (existingGiftCard == null) return NotFound();

        _mapper.Map(dto, existingGiftCard);

        var updatedGiftCard = await _giftCardService.UpdateGiftCardAsync(existingGiftCard);
        var giftCardResponseDTO = _mapper.Map<GiftCardResponseDTO>(updatedGiftCard);

        return Ok(giftCardResponseDTO);
    }

    [HttpDelete("{giftCardId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<IActionResult> DeleteGiftCard(int businessId, int giftCardId)
    {
        var success = await _giftCardService.DeleteGiftCardAsync(giftCardId, businessId);
        if (!success)
        {
            return NotFound();
        }

        return NoContent();
    }
}