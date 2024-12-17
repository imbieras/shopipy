using Shopipy.OrderManagement.DTOs.Discounts;
using Shopipy.Persistence.Models;

namespace Shopipy.OrderManagement.DTOs;

public class OrderDto
{
    public required int OrderId { get; set; }
    public required int BusinessId { get; set; }
    public required string UserId { get; set; }
    public required OrderStatus OrderStatus { get; set; }
    public required decimal TotalTip { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime? ClosedAt { get; set; }
    public required IEnumerable<OrderItemDto> OrderItems { get; set; }
    public required IEnumerable<DiscountDto> Discounts { get; set; }
}