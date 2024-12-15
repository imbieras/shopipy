using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.DiscountManagement.DTOs;
using Shopipy.Persistence.Models;
using Shopipy.Shared;
using Shopipy.Shared.Services;

namespace Shopipy.DiscountManagement.Controllers;

[ApiController]
[Route("businesses/{businessId}/discounts")]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class DiscountController(IDiscountService discountService, IMapper mapper) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscountResponseDto>>> GetAllDiscounts(int businessId)
    {
        var discounts = await discountService.GetAllDiscountsInBusinessAsync(businessId);

        return Ok(discounts.Select(mapper.Map<DiscountResponseDto>));
    }

    [HttpGet("{discountId}")]
    public async Task<ActionResult<DiscountResponseDto>> GetDiscountById(int businessId, int discountId)
    {
        var discount = await discountService.GetDiscountByIdInBusinessAsync(businessId, discountId);

        return Ok(mapper.Map<DiscountResponseDto>(discount));
    }
    
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<DiscountResponseDto>> CreateDiscount(
        int businessId,
        [FromBody] DiscountRequestDto discountRequestDto
    )
    {
        var discount = mapper.Map<Discount>(discountRequestDto);
        discount.BusinessId = businessId;

        discount = await discountService.CreateDiscountAsync(discount);

        return CreatedAtAction(nameof(GetDiscountById), new { businessId, discountId = discount.DiscountId }, mapper.Map<DiscountResponseDto>(discount));
    }

    [HttpPut("{discountId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<DiscountResponseDto>> UpdateDiscount(
        int businessId,
        int discountId,
        [FromBody] DateTime? effectiveTo
    )
    {
        var discount = await discountService.GetDiscountByIdInBusinessAsync(businessId, discountId);

        discount = await discountService.UpdateDiscountAsync(discount, effectiveTo);

        return Ok(mapper.Map<DiscountResponseDto>(discount));
    }

    [HttpDelete("{discountId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult> DeleteDiscount(int businessId, int discountId)
    {
        var success = await discountService.DeleteDiscountAsync(discountId);
        return success ? NoContent() : NotFound();
    }
}