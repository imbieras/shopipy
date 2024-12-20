using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Shopipy.Persistence.Models;

public class OrderPayment
{
    [Key]
    public int PaymentId { get; set; }
    public int BusinessId { get; set; }
    public int OrderId { get; set; }
    public required decimal AmountPaid { get; set; }
    public required PaymentMethod PaymentMethod { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public OrderPaymentStatus Status { get; set; }
    public string? StripePaymentId { get; set; }
    public string? StripePaymentIntentId { get; set; }
    public int? GiftCardId { get; set; }
    
    /* Navigational properties */
    public Business? Business { get; set; }
    public Order? Order { get; set; }
}