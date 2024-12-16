namespace Shopipy.Persistence.Models;

public class OrderDiscount
{
    public int OrderDiscountId { get; set; }
    public required int BusinessId { get; set; }
    public required int OrderId { get; set; }
    public int? OrderItemId { get; set; }
    public required int DiscountId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}