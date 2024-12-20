namespace Shopipy.OrderManagement.DTOs;

public class ReceiptDto
{
    public required List<int> PaymentIds { get; init; }
    public int OrderId { get; init; }
    public List<string> PaymentMethods { get; init; } = new List<string>();
    public decimal TipAmount { get; init; }
    public decimal TotalDiscount { get; init; }
    public decimal TaxAmount { get; init; }
    public decimal ServiceCharge { get; init; }
    public decimal TotalAmount { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? ClosedAt { get; init; }
    public IEnumerable<OrderItemReceiptDto> Items { get; init; } = Enumerable.Empty<OrderItemReceiptDto>();
}