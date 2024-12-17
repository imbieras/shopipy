using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.DiscountManagement.DTOs;
using Shopipy.Persistence.Models;
using Shopipy.Shared;
using Shopipy.Shared.Services;

namespace Shopipy.DiscountManagement.Controllers;

[ApiController]
[Route("businesses/{businessId:int}/discounts")]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
public class DiscountController(IDiscountService discountService, ICategoryService categoryService, IMapper mapper, ILogger<DiscountController> logger) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IEnumerable<DiscountResponseDto>>> GetAllDiscounts(int businessId)
    {
        var discounts = await discountService.GetAllDiscountsInBusinessAsync(businessId);

        return Ok(discounts.Select(mapper.Map<DiscountResponseDto>));
    }

    [HttpGet("{discountId:int}")]
    public async Task<ActionResult<DiscountResponseDto>> GetDiscountById(int businessId, int discountId)
    {
        var discount = await discountService.GetDiscountByIdInBusinessAsync(businessId, discountId);
        if (discount != null)
        {
            return Ok(mapper.Map<DiscountResponseDto>(discount));
        }

        logger.LogWarning("Discount with ID {DiscountId} in business {BusinessId} not found.", discountId, businessId);
        return NotFound();

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

        var category = await categoryService.GetCategoryByIdAsync(discount.CategoryId);

        if (category == null)
        {
            logger.LogWarning("Category with ID {CategoryId} not found for discount creation.", discount.CategoryId);
            return NotFound();
        }

        discount = await discountService.CreateDiscountAsync(discount);

        return CreatedAtAction(nameof(GetDiscountById), new { businessId, discountId = discount.DiscountId }, mapper.Map<DiscountResponseDto>(discount));
    }

    [HttpPut("{discountId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<DiscountResponseDto>> UpdateDiscount(
        int businessId,
        int discountId,
        [FromBody] DateTime? effectiveTo
    )
    {
        var discount = await discountService.GetDiscountByIdInBusinessAsync(businessId, discountId);
        if (discount == null)
        {
            logger.LogWarning("Discount with ID {DiscountId} in business {BusinessId} not found.", discountId, businessId);
            return NotFound();
        }

        discount = await discountService.UpdateDiscountAsync(discount, effectiveTo);

        return Ok(mapper.Map<DiscountResponseDto>(discount));
    }

    [HttpDelete("{discountId:int}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult> DeleteDiscount(int businessId, int discountId)
    {
        var existingDiscount = await discountService.GetDiscountByIdInBusinessAsync(businessId, discountId);
        if (existingDiscount == null)
        {
            logger.LogWarning("Discount with ID {CategoryId} in business {BusinessId} not found for deletion.", discountId, businessId);
            return NotFound();
        }
        
        var success = await discountService.DeleteDiscountAsync(discountId);

        if (success)
        {
            return NoContent();
        }

        logger.LogWarning("Discount with ID {DiscountId} in business {BusinessId} not found for deletion.", discountId, businessId);
        return NotFound();

    }
}