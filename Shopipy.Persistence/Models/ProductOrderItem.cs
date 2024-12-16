namespace Shopipy.Persistence.Models;

public class ProductOrderItem : OrderItem
{
    public required int ProductId { get; set; }
    public int? ProductVariationId { get; set; }
    public required int ProductQuantity { get; set; }
}