using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace Shopipy.Persistence.Models;

public class GiftCard
{
    [Key]
    public int GiftCardId { get; set; }

    [ForeignKey("Business")]
    public required int BusinessId { get; set; }

    [ForeignKey("Category")]
    public required int CategoryId { get; set; }

    public required decimal AmountOriginal { get; set; }

    public required decimal AmountLeft { get; set; }

    public required string GiftCardCode { get; set; }

    public required DateOnly ValidFrom { get; set; }

    public required DateOnly ValidUntil { get; set; }

    public required DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public required DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

}
