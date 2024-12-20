namespace Shopipy.OrderManagement.DTOs;

public class OrderItemReceiptDto
{
    public int ItemId { get; init; }
    public string ItemType { get; init; } = default!;
    public string Name { get; init; } = default!;
    public int Quantity { get; init; }
    public decimal UnitPrice { get; init; }
    public decimal DiscountAmount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal TotalPrice { get; init; }
}
