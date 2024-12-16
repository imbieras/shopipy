namespace Shopipy.OrderManagement.DTOs;

public class UpdateOrderItemDto
{
    public required int TaxRateId { get; set; }
    public int? ProductQuantity { get; set; }
}