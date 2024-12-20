using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shopipy.OrderManagement.DTOs;
using Shopipy.OrderManagement.DTOs.Discounts;
using Shopipy.OrderManagement.Services;
using Shopipy.Persistence.Data.Migrations;
using Shopipy.Persistence.Models;
using Shopipy.Shared;

namespace Shopipy.OrderManagement.Controllers;

[ApiController]
[Authorize(Policy = AuthorizationPolicies.RequireBusinessAccess)]
[Route("businesses/{businessId:int}/orders")]
public class OrdersController(OrderService orderService, IMapper mapper, ILogger<OrdersController> logger, PaymentService paymentService) : ControllerBase
{
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
    
    [HttpGet("{orderId:int}/product-items")]
    public async Task<ActionResult<IEnumerable<ProductOrderItemDto>>> GetProductOrderItems(int businessId, int orderId)
    {
        var productItems = await orderService.GetProductOrderItems(businessId, orderId);
        return Ok(mapper.Map<IEnumerable<ProductOrderItemDto>>(productItems));
    }


    [HttpGet("{orderId:int}/service-items")]
    public async Task<ActionResult<IEnumerable<ServiceOrderItemDto>>> GetServiceOrderItems(int businessId, int orderId)
    {
        var serviceItems = await orderService.GetServiceOrderItems(businessId, orderId);
        if (!serviceItems.Any()) return NotFound();
        return Ok(mapper.Map<IEnumerable<ServiceOrderItemDto>>(serviceItems));
    }

    [HttpGet("{orderId:int}/leftAmount")]
    public async Task<ActionResult<decimal>> GetLeftToPayForOrder(int businessId, int orderId)
    {
        var order = await orderService.GetOrderByIdAsync(businessId, orderId, withItems: false);
        if (order == null)
        {
            logger.LogWarning("Attempted to get non-existent order {OrderId} for business {BusinessId}.", orderId, businessId);
            return NotFound();
        }
        return Ok(await paymentService.AmountLeft(businessId, orderId));
    }

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

    [HttpPost("{orderId:int}/apply-discount")]
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

    [HttpDelete("{orderId:int}/discounts/{discountId:int}")]
    public async Task<IActionResult> DeleteDiscount(int businessId, int orderId, int discountId)
    {
        await orderService.DeleteDiscount(businessId, orderId, discountId);
        return NoContent();
    }

    [HttpGet("{orderId:int}/receipt")]
    public async Task<IActionResult> GetReceipt(int businessId, int orderId)
    {
        var order = await orderService.GetOrderByIdAsync(businessId, orderId);
        if (order is null)
        {
            return NotFound($"Order with ID {orderId} not found.");
        }

        if (order.OrderStatus != OrderStatus.Closed)
        {
            return Conflict("Receipt can only be retrieved for closed orders.");
        }

        var payments = await paymentService.GetPaymentsByOrderIdAsync(businessId, orderId);

        // Prepare the receipt DTO
        var receipt = new ReceiptDto
        {
            PaymentIds = payments.Select(payment => payment.PaymentId).ToList(),
            PaymentMethods = payments.Select(payment => payment.PaymentMethod.ToString()).ToList(),
            OrderId = order.OrderId,
            TipAmount = order.TotalTip, //do we have that
            TaxAmount = order.OrderItems?.Sum(i => i.TaxRateId.HasValue ? i.UnitPrice * 0.1m : 0) ?? 0, //not sure
            TotalAmount = await orderService.GetTotalPriceAsync(businessId, orderId),
            CreatedAt = order.CreatedAt,
            ClosedAt = order.ClosedAt,
            Items = order.OrderItems?.Select(item => new OrderItemReceiptDto
            {
                ItemId = item.OrderItemId,
                ItemType = item is ProductOrderItem ? "Product" : "Service", 
                Quantity = item is ProductOrderItem prodItem ? prodItem.ProductQuantity : 1, 
                UnitPrice = item.UnitPrice,
                TaxAmount = item.TaxRateId.HasValue ? item.UnitPrice * 0.1m : 0, // 
                TotalPrice = item.UnitPrice * (item is ProductOrderItem p ? p.ProductQuantity : 1) 
            })?.ToList() ?? new List<OrderItemReceiptDto>()
        };

        return Ok(receipt);
    }


}