using Shopipy.Persistence.Models;

namespace Shopipy.DiscountManagement.DTOs;

public class DiscountResponseDto
{
    public required int DiscountId { get; set; }

    public required int CategoryId { get; set; }

    public required int BusinessId { get; set; }

    public required string Name { get; set; }

    public required string Description { get; set; }

    public required decimal DiscountValue { get; set; }

    public required DiscountType DiscountType { get; set; }

    public required DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public required DateTime CreatedAt { get; set; }

    public required DateTime UpdatedAt { get; set; }
}