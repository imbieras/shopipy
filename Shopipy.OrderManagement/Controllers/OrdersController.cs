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
[Route("businesses/{businessId:int}/orders")]
public class OrdersController(OrderService orderService, IMapper mapper, ILogger<OrdersController> logger) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<OrderDto>> CreateOrder(int businessId, [FromBody] CreateOrderRequestDto request)
    {
        var order = await orderService.CreateOrderWithItemsAsync(businessId, request.UserId, request.OrderItems.Select(
        i => {
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

    [HttpGet("{orderId:int}")]
    public async Task<ActionResult<OrderDto>> GetOrderById(int businessId, int orderId)
    {
        var order = await orderService.GetOrderByIdAsync(businessId, orderId);
        if (order != null)
        {
            return Ok(mapper.Map<OrderDto>(order));
        }

        logger.LogWarning("Order {OrderId} not found for business {BusinessId}.", orderId, businessId);
        return NotFound();

    }

    [HttpPost("{orderId:int}/cancel")]
    public async Task<IActionResult> CancelOrder(int businessId, int orderId)
    {
        var order = await orderService.GetOrderByIdAsync(businessId, orderId, withItems: false);
        if (order == null)
        {
            logger.LogWarning("Attempted to cancel non-existent order {OrderId} for business {BusinessId}.", orderId, businessId);
            return NotFound();
        }

        await orderService.CancelOrderAsync(order);
        return Ok();
    }

    [HttpPost("{orderId:int}/items")]
    public async Task<ActionResult<OrderItemDto>> CreateOrderItem(
        int businessId,
        int orderId,
        [FromBody] CreateOrderItemRequestDto request
    )
    {
        var orderItem = mapper.Map<OrderItem>(request);
        orderItem.BusinessId = businessId;
        orderItem.OrderId = orderId;
        var result = await orderService.AddOrderItemAsync(orderItem);
        return CreatedAtAction(nameof(GetOrderById), new { businessId, orderId }, mapper.Map<OrderItemDto>(result));
    }

    [HttpPut("{orderId:int}/items/{orderItemId:int}")]
    public async Task<ActionResult<OrderItemDto>> UpdateOrderItem(
        int businessId,
        int orderId,
        int orderItemId,
        [FromBody] UpdateOrderItemDto request
    )
    {
        var orderItem = await orderService.GetOrderItemByIdAsync(businessId, orderId, orderItemId);

        switch (orderItem)
        {
            case null:
                logger.LogWarning("Attempted to update non-existent order item {OrderItemId} in order {OrderId} for business {BusinessId}.", orderItemId, orderId, businessId);
                return NotFound();
            case ProductOrderItem productOrderItem when request.ProductQuantity is not null:
                productOrderItem.ProductQuantity = request.ProductQuantity.Value;
                break;
        }

        var res = await orderService.UpdateOrderItemAsync(orderItem);
        return Ok(mapper.Map<OrderItemDto>(res));
    }

    [HttpDelete("{orderId:int}/items/{orderItemId:int}")]
    public async Task<IActionResult> DeleteOrderItem(int businessId, int orderId, int orderItemId)
    {
        await orderService.DeleteOrderItemAsync(businessId, orderId, orderItemId);
        return NoContent();
    }
}