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
    IGenericRepository<ProductOrderItem> productOrderItemRepository,
    IProductService productService,
    IProductVariationService productVariationService,
    IServiceManagementService serviceManagementService,
    ITaxService taxService)
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

    private async Task AddTaxRateToOrderItem(OrderItem orderItem, int categoryId)
    {
        var taxRate = (await taxService.GetAllTaxRatesByBusinessAsync(orderItem.BusinessId)).FirstOrDefault(t => t.CategoryId == categoryId && t.EffectiveFrom <= DateTime.UtcNow &&
            DateTime.UtcNow <= t.EffectiveTo);
        orderItem.TaxRateId = taxRate?.TaxRateId;
    }

    public async Task<OrderItem> AddOrderItemAsync(OrderItem orderItem, bool saveChanges = true)
    {
        var order = await GetOrderByIdAsync(orderItem.BusinessId, orderItem.OrderId);
        if (order is null)
        {
            throw new ArgumentException($"Order with id {orderItem.OrderId} not found");
        }

        if (order.OrderStatus != OrderStatus.Open)
        {
            throw new ArgumentException("Order is not open");
        }
        
        if (orderItem is ProductOrderItem productOrderItem)
        {
            var match = await productOrderItemRepository.GetByConditionAsync(i =>
                i.BusinessId == orderItem.BusinessId && i.OrderItemId == productOrderItem.OrderItemId &&
                i.OrderId == productOrderItem.OrderId && i.ProductId == productOrderItem.ProductId);
            if (match is not null)
            {
                throw new ArgumentException("Product is already included");
            }

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

            await AddTaxRateToOrderItem(productOrderItem, product.CategoryId);
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

        serviceOrderItem.UnitPrice = service.ServicePrice;
        await AddTaxRateToOrderItem(serviceOrderItem, service.CategoryId);

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

    public Task<OrderItem?> GetOrderItemByIdAsync(int businessId, int orderId, int orderItemId)
    {
        return orderItemRepository.GetByConditionAsync(i =>
            i.BusinessId == businessId && i.OrderId == orderId && i.OrderItemId == orderItemId);
    }

    public async Task<OrderItem> UpdateOrderItemAsync(OrderItem orderItem)
    {
        await ValidateOrderAsync(orderItem.BusinessId, orderItem.OrderId);
        return await orderItemRepository.UpdateAsync(orderItem);
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

    public async Task DeleteOrderItemAsync(int businessId, int orderId, int orderItemId)
    {
        await ValidateOrderAsync(businessId, orderId);
        await orderItemRepository.DeleteByConditionAsync(i =>
            i.BusinessId == businessId && i.OrderId == orderId && i.OrderItemId == orderItemId);
    }

    private async Task ValidateOrderAsync(int businessId, int orderId)
    {
        var order = await GetOrderByIdAsync(businessId, orderId);
        if (order is null)
        {
            throw new ArgumentException($"Order with id {orderId} not found");
        }

        if (order.OrderStatus != OrderStatus.Open)
        {
            throw new ArgumentException("Order is not open");
        }
    }
}