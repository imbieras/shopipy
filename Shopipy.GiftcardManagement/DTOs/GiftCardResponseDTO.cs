namespace Shopipy.GiftCardManagement.DTOs;

public class GiftCardResponseDTO
{
    public int GiftCardId { get; set; }
    public int BusinessId { get; set; }
    public int CategoryId { get; set; }
    public decimal AmountOriginal { get; set; }
    public decimal AmountLeft { get; set; }
    public required string GiftCardCode { get; set; }
    public DateOnly ValidFrom { get; set; }
    public DateOnly ValidUntil { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
