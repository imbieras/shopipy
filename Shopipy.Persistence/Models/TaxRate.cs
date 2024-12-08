using System.ComponentModel.DataAnnotations;

namespace Shopipy.Persistence.Models;

public class TaxRate
{
    [Key]
    public required int TaxRateId { get; set; }
    public required int CategoryId { get; set; }
    public required string Name { get; set; }
    public required decimal Rate { get; set; }
    public required DateTime EffectiveFrom { get; set; } 
    public DateTime? EffectiveTo { get; set; } // null if active
    public DateTime CreatedAt { get; set; }
    public required int BusinessId { get; set; }
}