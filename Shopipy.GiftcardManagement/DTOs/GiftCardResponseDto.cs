namespace Shopipy.GiftCardManagement.DTOs;

public class GiftCardResponseDto
{
    public int GiftCardId { get; init; }

    public int BusinessId { get; init; }

    public decimal AmountOriginal { get; init; }

    public decimal AmountLeft { get; init; }

    public required string GiftCardCode { get; init; }

    public DateOnly ValidFrom { get; init; }

    public DateOnly ValidUntil { get; init; }

    public DateTime CreatedAt { get; init; }

    public DateTime UpdatedAt { get; init; }
}