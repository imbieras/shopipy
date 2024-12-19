using Shopipy.Persistence.Models;

namespace Shopipy.OrderManagement.DTOs.Payments;

public class PaymentDto
{
    public int PaymentId { get; set; }
    public required decimal AmountPaid { get; set; }
    public required PaymentMethod PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderPaymentStatus Status { get; set; }
    public string? StripePaymentId { get; set; }
    public int? GiftCardId { get; set; }
}