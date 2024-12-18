namespace Shopipy.TaxManagement.DTOs;

public class TaxRateResponseDto
{
    public int TaxRateId { get; init; }

    public required string Name { get; init; }

    public required decimal Rate { get; init; }

    public required DateTime EffectiveFrom { get; init; }

    public DateTime? EffectiveTo { get; init; }

    public required DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }

    public int? BusinessId { get; init; }

    public required int CategoryId { get; init; }
}