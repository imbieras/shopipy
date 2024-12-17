using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.OrderManagement.DTOs;
using Shopipy.OrderManagement.DTOs.Discounts;
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
    public async Task<ActionResult<OrderDto>> CreateOrder(int businessId, [FromBody] CreateOrderRequestDto request)
    {
        var order = await orderService.CreateOrderWithItemsAsync(businessId, request.UserId, request.OrderItems.Select(
            i =>
            {
                var item = mapper.Map<OrderItem>(i);
                item.BusinessId = businessId;
                return item;
            }));
        return CreatedAtAction(nameof(GetOrderById), new { businessId, orderId = order.OrderId },
            mapper.Map<OrderDto>(order));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<OrderDto>>> GetOrders(int businessId)
    {
        var orders = await orderService.GetOrdersAsync(businessId);
        return Ok(mapper.Map<IEnumerable<OrderDto>>(orders));
    }

    [HttpGet("{orderId}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int businessId, int orderId)
    {
        var order = await orderService.GetOrderByIdAsync(businessId, orderId);
        if (order == null) return NotFound();
        return Ok(mapper.Map<OrderDto>(order));
    }

    [HttpPost("{orderId}/cancel")]
    public async Task<IActionResult> CancelOrder(int businessId, int orderId)
    {
        var order = await orderService.GetOrderByIdAsync(businessId, orderId, withItems: false);
        if (order == null) return NotFound();
        await orderService.CancelOrderAsync(order);
        return Ok();
    }

    [HttpPost("{orderId}/items")]
    public async Task<ActionResult<OrderItemDto>> CreateOrderItem(int businessId, int orderId,
        [FromBody] CreateOrderItemRequestDto request)
    {
        var orderItem = mapper.Map<OrderItem>(request);
        orderItem.BusinessId = businessId;
        orderItem.OrderId = orderId;
        var result = await orderService.AddOrderItemAsync(orderItem);
        return CreatedAtAction(nameof(GetOrderById), new { businessId, orderId }, mapper.Map<OrderItemDto>(result));
    }
    
    [HttpPut("{orderId}/items/{orderItemId}")]
    public async Task<ActionResult<OrderItemDto>> UpdateOrderItem(int businessId, int orderId, int orderItemId,
        [FromBody] UpdateOrderItemDto request)
    {
        var orderItem = await orderService.GetOrderItemByIdAsync(businessId, orderId, orderItemId);
        if (orderItem == null) return NotFound();
        if (orderItem is ProductOrderItem productOrderItem && request.ProductQuantity is not null)
        {
            productOrderItem.ProductQuantity = request.ProductQuantity.Value;
        }

        var res = await orderService.UpdateOrderItemAsync(orderItem);
        return Ok(mapper.Map<OrderItemDto>(res));
    }
    
    [HttpDelete("{orderId}/items/{orderItemId}")]
    public async Task<IActionResult> DeleteOrderItem(int businessId, int orderId, int orderItemId)
    {
        await orderService.DeleteOrderItemAsync(businessId, orderId, orderItemId);
        return NoContent();
    }

    [HttpPost("{orderId}/apply-discount")]
    public async Task<ActionResult<OrderDto>> CreateDiscount(int businessId, int orderId, [FromBody] ApplyDiscountRequestDto request)
    {
        var order = await orderService.GetOrderByIdAsync(businessId, orderId);
        if (order == null) return NotFound();
        if (request.ApplyTo == ApplyTo.Order)
        {
            await orderService.ApplyDiscount(order, request.Discountid);
        }
        else
        {
            if (request.OrderItemIds == null || order.OrderItems == null) return new BadRequestResult();
            await orderService.ApplyDiscount(order.OrderItems.Where(i => request.OrderItemIds.Contains(i.OrderItemId)), request.Discountid);
        }
        return Ok(mapper.Map<OrderDto>(await orderService.GetOrderByIdAsync(businessId, orderId)));
    }

    [HttpDelete("{orderId}/discounts/{discountId}")]
    public async Task<IActionResult> DeleteDiscount(int businessId, int orderId, int discountId)
    {
        await orderService.DeleteDiscount(businessId, orderId, discountId);
        return NoContent();
    }
}