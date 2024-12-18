using Shopipy.Persistence.Models;

namespace Shopipy.DiscountManagement.DTOs;

public class DiscountResponseDto
{
    public required int DiscountId { get; init; }

    public required int CategoryId { get; init; }

    public required int BusinessId { get; init; }

    public required string Name { get; init; }

    public required string Description { get; init; }

    public required decimal DiscountValue { get; init; }

    public required DiscountType DiscountType { get; init; }

    public required DateTime EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime UpdatedAt { get; init; }
}