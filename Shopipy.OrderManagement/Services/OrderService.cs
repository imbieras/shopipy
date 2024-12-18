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
    IGenericRepository<ServiceOrderItem> serviceOrderItemRepository,
    IProductService productService,
    IProductVariationService productVariationService,
    IServiceManagementService serviceManagementService,
    ITaxService taxService,
    IDiscountService discountService,
    IGenericRepository<OrderDiscount> orderDiscountRepository)
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
        await orderRepository.SaveChangesAsync();

        foreach (var orderItem in orderItems)
        {
            orderItem.OrderId = order.OrderId;
            await AddOrderItemAsync(orderItem, saveChanges: false);
        }

        await orderRepository.SaveChangesAsync();
        return await GetOrderByIdAsync(businessId, order.OrderId) ?? 
               throw new InvalidOperationException("Created order not found");
    }

    private async Task AddTaxRateToOrderItem(OrderItem orderItem, int categoryId)
    {
        var taxRate = (await taxService.GetAllTaxRatesByBusinessAsync(orderItem.BusinessId)).FirstOrDefault(t =>
            t.CategoryId == categoryId && t.EffectiveFrom <= DateTime.UtcNow &&
            DateTime.UtcNow <= t.EffectiveTo);
        orderItem.TaxRateId = taxRate?.TaxRateId;
    }

    public async Task<OrderItem> AddOrderItemAsync(OrderItem orderItem, bool saveChanges = true)
    {
        if (saveChanges)
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
                await productService.GetProductByIdInBusinessAsync(productOrderItem.ProductId, productOrderItem.BusinessId);
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
                if (!saveChanges) return item;
                await UpdateOrderTime(orderItem.BusinessId, orderItem.OrderId);
                await orderItemRepository.SaveChangesAsync();
                return item;
            }

            var variation = await productVariationService.GetVariationByIdInBusinessAsync(
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
            await UpdateOrderTime(orderItem.BusinessId, orderItem.OrderId);
            productOrderItem.UnitPrice += variation.PriceModifier;
            var item2 = await orderItemRepository.AddWithoutSavingChangesAsync(productOrderItem);
            if (!saveChanges) return item2;
            await UpdateOrderTime(orderItem.BusinessId, orderItem.OrderId);
            await orderItemRepository.SaveChangesAsync();
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
        await UpdateOrderTime(orderItem.BusinessId, orderItem.OrderId);
        var item3 = await orderItemRepository.AddWithoutSavingChangesAsync(serviceOrderItem);
        if (!saveChanges) return item3;
        await UpdateOrderTime(orderItem.BusinessId, orderItem.OrderId);
        await orderItemRepository.SaveChangesAsync();
        return item3;
    }

    public Task<Order?> GetOrderByIdAsync(int businessId, int orderId, bool withItems = true)
    {
        return withItems ? orderRepository.GetOrderByIdWithItemsAsync(businessId, orderId) : orderRepository.GetByConditionAsync(o => o.BusinessId == businessId && o.OrderId == orderId);
    }

    public Task<IEnumerable<ProductOrderItem>> GetProductOrderItems(int businessId, int orderId)
    {
        return productOrderItemRepository.GetAllByConditionAsync(o => o.BusinessId == businessId && o.OrderId == orderId);
    }
    
    public Task<IEnumerable<ServiceOrderItem>> GetServiceOrderItems(int businessId, int orderId)
    {
        return serviceOrderItemRepository.GetAllByConditionAsync(o => o.BusinessId == businessId && o.OrderId == orderId);
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
        await UpdateOrderTime(orderItem.BusinessId, orderItem.OrderId);
        return await orderItemRepository.UpdateAsync(orderItem);
    }

    public async Task<Order> CancelOrderAsync(Order order)
    {
        if (order.OrderStatus != OrderStatus.Open)
        {
            throw new ArgumentException($"Order with id {order.OrderId} is not open");
        }

        order.OrderStatus = OrderStatus.Cancelled;
        order.UpdatedAt = DateTime.UtcNow;
        await orderRepository.UpdateAsync(order);
        return order;
    }

    public async Task DeleteOrderItemAsync(int businessId, int orderId, int orderItemId)
    {
        await ValidateOrderAsync(businessId, orderId);
        await orderItemRepository.DeleteByConditionAsync(i =>
            i.BusinessId == businessId && i.OrderId == orderId && i.OrderItemId == orderItemId);
        await UpdateOrderTime(businessId, orderId);
    }

    public async Task ApplyDiscount(Order order, int discountId)
    {
        await ValidateOrderAsync(order.BusinessId, order.OrderId);
        await orderDiscountRepository.AddWithoutSavingChangesAsync(new OrderDiscount
        {
            OrderId = order.OrderId,
            BusinessId = order.BusinessId,
            DiscountId = discountId
        });
        await UpdateOrderTime(order.BusinessId, order.OrderId, saveChanges: false);
        await orderRepository.SaveChangesAsync();
    }

    public async Task ApplyDiscount(IEnumerable<OrderItem> orderItems, int discountId)
    {
        foreach (var orderItem in orderItems)
        {
            await ValidateOrderAsync(orderItem.BusinessId, orderItem.OrderId);
            await orderDiscountRepository.AddWithoutSavingChangesAsync(new OrderDiscount
            {
                OrderId = orderItem.OrderId,
                BusinessId = orderItem.BusinessId,
                DiscountId = discountId,
                OrderItemId = orderItem.OrderItemId
            });
            await UpdateOrderTime(orderItem.BusinessId, orderItem.OrderId, saveChanges: false);
        }
        await orderRepository.SaveChangesAsync();
    }

    public async Task DeleteDiscount(int businessId, int orderId, int discountId)
    {
        await ValidateOrderAsync(businessId, orderId);
        await orderDiscountRepository.DeleteAsync(discountId);
        await UpdateOrderTime(businessId, orderId);
    }

    public async Task<decimal> GetTotalPriceAsync(int businessId, int orderId)
    {
        var order = await GetOrderByIdAsync(businessId, orderId);
        if (order == null) throw new ArgumentException("Order not found");
        decimal totalPrice = 0;
        foreach (var orderItem in order.OrderItems)
        {
            var orderItemPrice = orderItem.UnitPrice;
            if (orderItem is ProductOrderItem productOrderItem)
            {
                orderItemPrice *= productOrderItem.ProductQuantity;
            }

            foreach (var orderItemOrderDiscount in orderItem.OrderDiscounts)
            {
                orderItemPrice = await CalculateDiscountAsync(orderItemPrice, orderItemOrderDiscount);
            }

            if (orderItem.TaxRateId is not null)
            {
                var tax = await taxService.GetTaxRateByIdAndBusinessAsync(orderItem.TaxRateId.Value, orderItem.BusinessId);
                if (tax != null) orderItemPrice *= (1 - tax.Rate);
            }
            
            totalPrice += orderItemPrice;
        }
        
        foreach (var orderDiscount in order.OrderDiscounts)
        {
            totalPrice = await CalculateDiscountAsync(totalPrice, orderDiscount);
        }
        
        return totalPrice;
    }

    private async Task<decimal> CalculateDiscountAsync(decimal currentPrice, OrderDiscount orderDiscount)
    {
        var discount = await discountService.GetDiscountByIdAsync(orderDiscount.DiscountId);
        if (discount == null) return currentPrice;
        switch (discount.DiscountType)
        {
            case DiscountType.Fixed:
                currentPrice -= discount.DiscountValue;
                break;
            case DiscountType.Percentage:
                currentPrice *= (1 - discount.DiscountValue);
                break;
            default:
                throw new InvalidOperationException($"Unknown discount type: {discount.DiscountType}");
        }
        return currentPrice;
    }

    public async Task ValidateOrderAsync(int businessId, int orderId)
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

    private async Task UpdateOrderTime(int businessId, int orderId, bool saveChanges = true)
    {
        var order = await GetOrderByIdAsync(businessId, orderId);
        if (order is null) return;
        order.UpdatedAt = DateTime.UtcNow;
        if (saveChanges) await orderRepository.UpdateAsync(order);
    }
}