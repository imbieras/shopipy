using System.ComponentModel.DataAnnotations;

namespace Shopipy.OrderManagement.DTOs.Discounts;

public enum ApplyTo
{
    Order,
    Items
}

public class ApplyDiscountRequestDto : IValidatableObject
{
    public int Discountid { get; set; }
    public ApplyTo ApplyTo { get; set; }
    public IEnumerable<int>? OrderItemIds { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if ((ApplyTo == ApplyTo.Order) != (OrderItemIds is null))
            yield return new ValidationResult("OrderItemIds must be set only if ApplyTo is not Order.");
    }
}