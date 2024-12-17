namespace Shopipy.TaxManagement.DTOs;

public class TaxRateRequestDto
{
    public required string Name { get; set; }

    public required decimal Rate { get; set; }

    public required DateTime EffectiveFrom { get; set; }

    public DateTime? EffectiveTo { get; set; }

    public required int CategoryId { get; set; }
}