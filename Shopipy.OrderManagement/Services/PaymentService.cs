using Shopipy.Persistence.Models;
using Shopipy.Persistence.Repositories;
using Stripe;
using PaymentMethod = Shopipy.Persistence.Models.PaymentMethod;

namespace Shopipy.OrderManagement.Services;

public class PaymentService(IGenericRepository<OrderPayment> paymentRepository, OrderService orderService)
{
    public async Task<OrderPayment> CreatePaymentAsync(OrderPayment orderPayment)
    {
        if (orderPayment is { PaymentMethod: PaymentMethod.GiftCard, GiftCardId: null })
        {
            throw new ArgumentException("Gift card id not provided");
        }

        await orderService.ValidateOrderAsync(orderPayment.BusinessId, orderPayment.OrderId);

        orderPayment.Status = OrderPaymentStatus.Pending;
        if (orderPayment.PaymentMethod == PaymentMethod.Cash) orderPayment.Status = OrderPaymentStatus.Succeeded;
        if (orderPayment.PaymentMethod == PaymentMethod.Card)
        {
            var (clientSecret, paymentIntentId) = await ProcessStripePaymentAsync(orderPayment);

            orderPayment.StripePaymentId = clientSecret;       
            orderPayment.StripePaymentIntentId = paymentIntentId;
        }

        var createdPayment = await paymentRepository.AddAsync(orderPayment);

        // TODO: close order after enough payments

        return createdPayment;
    }

    // TODO: update payment status
    public async Task<IEnumerable<OrderPayment>> GetPaymentsByOrderIdAsync(int businessId, int orderId)
    {
        return await paymentRepository.GetAllByConditionAsync(p => p.BusinessId == businessId && p.OrderId == orderId);
    }

    public async Task<OrderPayment?> GetPaymentById(int businessId, int orderId, int paymentId)
    {
        return await paymentRepository.GetByConditionAsync(p =>
            p.BusinessId == businessId && p.OrderId == orderId && p.PaymentId == paymentId);
    }

    private async Task<(string ClientSecret, string PaymentIntentId)> ProcessStripePaymentAsync(OrderPayment orderPayment)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(orderPayment.AmountPaid * 100), // Convert to cents
            Currency = "usd",
            PaymentMethodTypes = ["card"],
        };

        var service = new PaymentIntentService();

        var paymentIntent = await service.CreateAsync(options);

        return (paymentIntent.ClientSecret, paymentIntent.Id); 
    }

    public async Task<string> RefundStripePaymentAsync(OrderPayment orderPayment, string reason)
    {
        var options = new RefundCreateOptions
        {
            Amount = (long)(orderPayment.AmountPaid * 100),
            Reason = reason,
            PaymentIntent = orderPayment.StripePaymentIntentId,
        };

        var service = new RefundService();

        var refund = await service.CreateAsync(options);

        if (refund.Status == "succeeded")
        {
            return "completed";
        }
        return "failed";
    }

    public async Task<OrderPayment> RefundPaymentAsync(OrderPayment payment, string reason)
    {
        var order = await orderService.GetOrderByIdAsync(payment.BusinessId, payment.OrderId);

        if (order.OrderStatus != OrderStatus.Closed)
        {
            throw new ArgumentException($"Order with id {order.OrderId} is not closed");
        }
        

        if (payment.PaymentMethod == PaymentMethod.Card)
        {
            await RefundStripePaymentAsync(payment, reason);
        }
        else if (payment.PaymentMethod == PaymentMethod.GiftCard)
        {
            throw new ArgumentException($"Order with id {order.OrderId} is not closed");
        }

        order.OrderStatus = OrderStatus.Refunded;
        order.UpdatedAt = DateTime.UtcNow;

        //await orderService.UpdateOrderItemAsync(order);
        return payment;
    }
}