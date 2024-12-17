using Shopipy.OrderManagement.DTOs.Discounts;

namespace Shopipy.OrderManagement.DTOs;

public abstract class OrderItemDto
{
    public int OrderItemId { get; init; }

    public required int OrderId { get; init; }

    public required decimal UnitPrice { get; init; }

    public int? TaxRateId { get; init; }

    public DateTime CreatedAt { get; init; }

    public IEnumerable<DiscountDto> Discounts { get; init; }
}