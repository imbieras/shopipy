namespace Shopipy.OrderManagement.DTOs;

public class CreateServiceOrderItemRequestDto : CreateOrderItemRequestDto
{
    public required int ServiceId { get; set; }
}