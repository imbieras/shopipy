using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shopipy.Persistence.Models;

public class ProductVariation
{
    [Key]
    public int VariationId { get; set; }

    [ForeignKey("Product")]
    public required int ProductId { get; set; }

    [Required]
    [MaxLength(255)]
    public required string Name { get; set; }

    public required decimal PriceModifier { get; set; }

    public required ProductState ProductState { get; set; }

    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}
