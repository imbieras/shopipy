namespace Shopipy.OrderManagement.DTOs;

public class CreateProductOrderItemRequestDto : CreateOrderItemRequestDto
{
    public required int ProductId { get; set; }
    public int? ProductVariationId { get; set; }
    public required int ProductQuantity { get; set; }
}