namespace Shopipy.OrderManagement.DTOs;

public class ProductOrderItemDto : OrderItemDto
{
    public required int ProductId { get; set; }
    public int? ProductVariationId { get; set; }
    public required int ProductQuantity { get; set; }
}