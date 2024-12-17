namespace Shopipy.OrderManagement.DTOs;

public class ProductOrderItemDto : OrderItemDto
{
    public required int ProductId { get; init; }

    public int? ProductVariationId { get; init; }

    public required int ProductQuantity { get; init; }
}