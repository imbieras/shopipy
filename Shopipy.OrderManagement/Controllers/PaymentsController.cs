using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Shopipy.OrderManagement.DTOs.Payments;
using Shopipy.OrderManagement.Services;
using Shopipy.Persistence.Models;

namespace Shopipy.OrderManagement.Controllers;

[ApiController]
[Route("businesses/{businessId:int}/orders/{orderId:int}/payments")]
public class PaymentsController(IMapper mapper, PaymentService paymentService, ILogger<PaymentsController> logger) : ControllerBase {

    [HttpPost]
    public async Task<ActionResult<PaymentDto>> CreatePayment(int businessId, int orderId, [FromBody] CreatePaymentRequestDto request)
    {
        var payment = mapper.Map<OrderPayment>(request);
        payment.BusinessId = businessId;
        payment.OrderId = orderId;
        var created = await paymentService.CreatePaymentAsync(payment);
        return CreatedAtAction(nameof(GetPayment), new { orderId, businessId, paymentId = created.PaymentId }, mapper.Map<PaymentDto>(created));
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PaymentDto>>> GetPayments(int businessId, int orderId)
    {
        return Ok(mapper.Map<IEnumerable<PaymentDto>>(await paymentService.GetPaymentsByOrderIdAsync(businessId, orderId)));
    }
    
    [HttpGet("{paymentId:int}")]
    public async Task<ActionResult<PaymentDto>> GetPayment(int businessId, int orderId, int paymentId)
    {
        return Ok(mapper.Map<PaymentDto>(await paymentService.GetPaymentById(businessId, orderId, paymentId)));
    }

    [HttpPost("{paymentId:int}/refund")]
    public async Task<IActionResult> RefundPayment(int businessId, int orderId, int paymentId, RefundDto refund)
    {
        var payment = await paymentService.GetPaymentById(businessId, orderId, paymentId);
        if (payment == null)
        {
            logger.LogWarning("Attempted to refund non-existent payment {PaymentId} for business {BusinessId}.", paymentId, businessId);
            return NotFound();
        }

        await paymentService.RefundPaymentAsync(payment, refund.RefundReason);
        return Ok();
    }
}