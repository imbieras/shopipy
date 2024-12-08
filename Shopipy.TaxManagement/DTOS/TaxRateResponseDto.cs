namespace Shopipy.TaxManagement.DTOs;

public class TaxRateResponseDto
{
    public int TaxRateId { get; set; }
    public required string Name { get; set; }
    public required decimal Rate { get; set; }
    public required DateTime EffectiveFrom { get; set; }
    public DateTime? EffectiveTo { get; set; }
    public required DateTime CreatedAt { get; set; }
    public int? BusinessId { get; set; }
    public required int CategoryId { get; set; }
}