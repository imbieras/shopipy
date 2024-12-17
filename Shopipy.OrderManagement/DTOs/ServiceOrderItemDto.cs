namespace Shopipy.OrderManagement.DTOs;

public class ServiceOrderItemDto : OrderItemDto
{
    public required int ServiceId { get; init; }
}