using Shopipy.Persistence.Models;

namespace Shopipy.OrderManagement.DTOs.Payments;

public class CreatePaymentRequestDto
{
    public required decimal AmountPaid { get; set; }
    public required PaymentMethod PaymentMethod { get; set; }
    public int? GiftCardId { get; set; }
}