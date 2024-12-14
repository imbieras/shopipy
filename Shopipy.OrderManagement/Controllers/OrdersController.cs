using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.OrderManagement.DTOs;
using Shopipy.OrderManagement.Services;
using Shopipy.Persistence.Models;
using Shopipy.Shared;

namespace Shopipy.OrderManagement.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
[Route("businesses/{businessId}/orders")]
public class OrdersController(OrderService orderService, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult> CreateOrder(int businessId, [FromBody] CreateOrderRequestDto request)
    {
        var order = await orderService.CreateOrderWithItemsAsync(businessId, request.UserId, request.OrderItems.Select(
            i =>
            {
                var item = mapper.Map<OrderItem>(i);
                item.BusinessId = businessId;
                return item;
            }));
        return Ok();
    }
}