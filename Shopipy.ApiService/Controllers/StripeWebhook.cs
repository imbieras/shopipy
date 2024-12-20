using Microsoft.AspNetCore.Mvc;
using Shopipy.OrderManagement.Services;
using Stripe;

namespace Shopipy.ApiService.Controllers;

[Route("stripe-webhook")]
public class StripeWebHook(PaymentService paymentService, ILogger<StripeWebHook> logger) : Controller
{
    [HttpPost]
    public async Task<IActionResult> Index()
    {
        var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();

        try
        {
            var stripeEvent = EventUtility.ParseEvent(json);

            // Handle the event
            // If on SDK version < 46, use class Events instead of EventTypes
            if (stripeEvent.Type == EventTypes.PaymentIntentSucceeded)
            {
                var paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                if (paymentIntent == null)
                {
                    logger.LogInformation("PaymentIntent is unexpectedly null");
                    return Ok();
                }
                await paymentService.CompleteStripePaymentAsync(paymentIntent.ClientSecret);
                // Then define and call a method to handle the successful payment intent.
                // handlePaymentIntentSucceeded(paymentIntent);
            }
            else if (stripeEvent.Type == EventTypes.PaymentMethodAttached)
            {
                var paymentMethod = stripeEvent.Data.Object as PaymentMethod;
                // Then define and call a method to handle the successful attachment of a PaymentMethod.
                // handlePaymentMethodAttached(paymentMethod);
            }
            // ... handle other event types
            else
            {
                // Unexpected event type
                Console.WriteLine("Unhandled event type: {0}", stripeEvent.Type);
            }
            return Ok();
        }
        catch (StripeException e)
        {
            return BadRequest();
        }
    }
}