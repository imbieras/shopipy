namespace Shopipy.GiftCardManagement.DTOs;

public class GiftCardRequestDTO
{
    public required decimal AmountOriginal { get; set; }
    public required DateOnly ValidFrom { get; set; }
    public required DateOnly ValidUntil { get; set; }
}
