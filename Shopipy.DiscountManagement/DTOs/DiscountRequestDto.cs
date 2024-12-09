using Shopipy.Persistence.Models;
using System.ComponentModel.DataAnnotations;

namespace Shopipy.DiscountManagement.DTOs;

public class DiscountRequestDto
{
    public required int CategoryId { get; set; }

    [StringLength(255)]
    public required string Name { get; set; }

    [StringLength(255)]
    public required string Description { get; set; }

    public required decimal DiscountValue { get; set; }

    public required DiscountType DiscountType { get; set; }

    public required DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }
}