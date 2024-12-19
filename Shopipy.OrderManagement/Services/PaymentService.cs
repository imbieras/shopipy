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
            orderPayment.StripePaymentId = await ProcessStripePaymentAsync(orderPayment);
        }

        var createdPayment = await paymentRepository.AddAsync(orderPayment);

        // TODO: close order after enough payments

        return createdPayment;
    }

    public async Task<IEnumerable<OrderPayment>> GetPaymentsByOrderIdAsync(int businessId, int orderId)
    {
        return await paymentRepository.GetAllByConditionAsync(p => p.BusinessId == businessId && p.OrderId == orderId);
    }

    public async Task<OrderPayment?> GetPaymentById(int businessId, int orderId, int paymentId)
    {
        return await paymentRepository.GetByConditionAsync(p =>
            p.BusinessId == businessId && p.OrderId == orderId && p.PaymentId == paymentId);
    }

    private async Task<string> ProcessStripePaymentAsync(OrderPayment orderPayment)
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(orderPayment.AmountPaid * 100), // Convert to cents
            Currency = "usd",
            PaymentMethodTypes = ["card"],
        };

        var service = new PaymentIntentService();
        
        var paymentIntent = await service.CreateAsync(options);

        return paymentIntent.ClientSecret;
    }
}