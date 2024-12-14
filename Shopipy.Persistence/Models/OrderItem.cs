namespace Shopipy.Persistence.Models;

public class OrderItem
{
    public int OrderItemId { get; set; }
    public required int BusinessId { get; set; }
    public required int OrderId { get; set; }
    public int? ProductId { get; set; }
    public int? ProductVariationId { get; set; }
    public int ProductQuantity { get; set; }
    public int? ServiceId { get; set; }
    public required decimal UnitPrice { get; set; }
    public int? TaxRateId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
}