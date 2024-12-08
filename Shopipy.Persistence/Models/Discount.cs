using System.ComponentModel.DataAnnotations;

namespace Shopipy.Persistence.Models;

public class Discount
{
    [Key]
    public required int DiscountId { get; set; }

    public required int CategoryId { get; set; }

    public required int BusinessId { get; set; }

    [StringLength(255)]
    public required string Name { get; set; }

    [StringLength(255)]
    public required string Description { get; set; }

    public required decimal DiscountValue { get; set; }

    public required DiscountType DiscountType { get; set; }

    public required DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }// Null if the discount is permanent

    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}