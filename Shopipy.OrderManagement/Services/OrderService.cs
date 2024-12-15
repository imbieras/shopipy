using Shopipy.OrderManagement.Repositories;
using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Shopipy.Shared.Services;
using Shopipy.UserManagement.Services;

namespace Shopipy.OrderManagement.Services;

public class OrderService(
    UserService userService,
    OrderRepository orderRepository,
    IGenericRepository<OrderItem> orderItemRepository,
    IProductService productService,
    IProductVariationService productVariationService,
    IServiceManagementService serviceManagementService)
{
    public async Task<Order> CreateOrderWithItemsAsync(int businessId, string userId, IEnumerable<OrderItem> orderItems)
    {
        var user = await userService.GetUserByIdAsync(userId);
        if (user is null)
        {
            throw new ArgumentException($"User with id {userId} not found");
        }

        var order = await orderRepository.AddWithoutSavingChangesAsync(new Order
            { BusinessId = businessId, UserId = userId, OrderStatus = OrderStatus.Open });
        foreach (var orderItem in orderItems)
        {
            orderItem.OrderItemId = order.OrderId;
            await AddOrderItemAsync(orderItem, saveChanges: false);
        }

        await orderRepository.SaveChangesAsync();
        return order;
    }

    public async Task<OrderItem> AddOrderItemAsync(OrderItem orderItem, bool saveChanges = true)
    {
        // TODO: check if there are order items with same product id and disallow
        if (orderItem is ProductOrderItem productOrderItem)
        {
            var product =
                await productService.GetProductByIdAsync(productOrderItem.ProductId, productOrderItem.BusinessId);
            if (product is null)
            {
                throw new ArgumentException($"Product with id {productOrderItem.ProductId} not found");
            }

            if (product.ProductState != ProductState.Available)
            {
                throw new ArgumentException($"Product with id {productOrderItem.ProductId} is not available");
            }

            productOrderItem.UnitPrice = product.BasePrice;
            if (productOrderItem.ProductVariationId is null)
            {
                var item = await orderItemRepository.AddWithoutSavingChangesAsync(productOrderItem);
                if (saveChanges) await orderItemRepository.SaveChangesAsync();
                return item;
            }

            var variation = await productVariationService.GetVariationByIdAsync(
                productOrderItem.ProductVariationId.Value,
                productOrderItem.ProductId, productOrderItem.BusinessId);
            if (variation is null)
            {
                throw new ArgumentException(
                    $"Variation with id {productOrderItem.ProductVariationId.Value} not found");
            }

            if (variation.ProductState != ProductState.Available)
            {
                throw new ArgumentException(
                    $"Product variation with id {productOrderItem.ProductId} is not available");
            }

            productOrderItem.UnitPrice += variation.PriceModifier;
            var item2 = await orderItemRepository.AddWithoutSavingChangesAsync(productOrderItem);
            if (saveChanges) await orderItemRepository.SaveChangesAsync();
            return item2;
        }

        if (orderItem is not ServiceOrderItem serviceOrderItem) throw new ArgumentException("Invalid order item type");
        var service =
            await serviceManagementService.GetServiceByIdInBusiness(serviceOrderItem.BusinessId,
                serviceOrderItem.ServiceId);
        if (service is null)
        {
            throw new ArgumentException($"Service with id {serviceOrderItem.ServiceId} is not found");
        }

        if (!service.IsServiceActive)
        {
            throw new ArgumentException($"Service with id {serviceOrderItem.ServiceId} is not active");
        }

        serviceOrderItem.UnitPrice = service.ServiceBasePrice;

        var item3 = await orderItemRepository.AddWithoutSavingChangesAsync(serviceOrderItem);
        if (saveChanges) await orderItemRepository.SaveChangesAsync();
        return item3;
    }

    public Task<Order?> GetOrderByIdAsync(int businessId, int orderId, bool withItems = true)
    {
        if (withItems)
        {
            return orderRepository.GetOrderByIdWithItemsAsync(businessId, orderId);
        }

        return orderRepository.GetByConditionAsync(o => o.BusinessId == businessId && o.OrderId == orderId);
    }

    public Task<IEnumerable<Order>> GetOrdersAsync(int businessId)
    {
        return orderRepository.GetOrdersWithItemsAsync(businessId);
    }

    public async Task<Order> CancelOrderAsync(Order order)
    {
        if (order.OrderStatus != OrderStatus.Open)
        {
            throw new ArgumentException($"Order with id {order.OrderId} is not open");
        }

        order.OrderStatus = OrderStatus.Cancelled;
        await orderRepository.UpdateAsync(order);
        return order;
    }

    public Task DeleteOrderItemAsync(int businessId, int orderId, int orderItemId)
    {
        return orderItemRepository.DeleteByConditionAsync(i =>
            i.BusinessId == businessId && i.OrderId == orderId && i.OrderItemId == orderItemId);
    }
}