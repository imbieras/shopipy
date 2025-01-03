namespace Shopipy.Persistence.Models;

public class Order
{
    public int OrderId { get; set; }
    public required int BusinessId { get; set; }
    public required string UserId { get; set; }
    public required OrderStatus OrderStatus { get; set; }
    public decimal TotalTip { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? ClosedAt { get; set; }
    public IEnumerable<OrderItem>? OrderItems { get; set; }
    public IEnumerable<OrderDiscount>? OrderDiscounts { get; set; }
    
    public Order Clone() => (Order)MemberwiseClone();
}