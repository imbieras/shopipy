namespace Shopipy.Persistence.Models;

public abstract class OrderItem
{
    public int OrderItemId { get; set; }
    public required int BusinessId { get; set; }
    public required int OrderId { get; set; }
    public required decimal UnitPrice { get; set; }
    public int? TaxRateId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}