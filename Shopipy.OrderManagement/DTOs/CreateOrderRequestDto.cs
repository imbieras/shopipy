namespace Shopipy.OrderManagement.DTOs;

public class CreateOrderRequestDto
{
    public required IEnumerable<CreateOrderItemRequestDto> OrderItems { get; set; }

    public required string UserId { get; set; }
}