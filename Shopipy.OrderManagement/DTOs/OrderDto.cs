using Shopipy.OrderManagement.DTOs.Discounts;
using Shopipy.Persistence.Models;

namespace Shopipy.OrderManagement.DTOs;

public class OrderDto
{
    public required int OrderId { get; init; }

    public required int BusinessId { get; init; }

    public required string UserId { get; init; }

    public required OrderStatus OrderStatus { get; init; }

    public required decimal TotalTip { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public DateTime? ClosedAt { get; init; }

    public required IEnumerable<OrderItemDto> OrderItems { get; init; }

    public required IEnumerable<DiscountDto> Discounts { get; init; }
}