namespace Shopipy.OrderManagement.DTOs;

public abstract class OrderItemDto
{
    public int OrderItemId { get; set; }
    public required int OrderId { get; set; }
    public required decimal UnitPrice { get; set; }
    public int? TaxRateId { get; set; }
    public DateTime CreatedAt { get; set; }
}