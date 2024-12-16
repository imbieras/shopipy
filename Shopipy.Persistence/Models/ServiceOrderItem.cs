namespace Shopipy.Persistence.Models;

public class ServiceOrderItem : OrderItem
{
    public required int ServiceId { get; set; }
}