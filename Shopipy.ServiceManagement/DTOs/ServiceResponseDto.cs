namespace Shopipy.ServiceManagement.Mappings;

public class ServiceResponseDto
{
    //Should also return category ID
    public required int ServiceId { get; set; }
    public required string ServiceName { get; set; }
    public required string ServiceDescription { get; set; }
    public required decimal ServiceBasePrice { get; set; }
    public required int ServiceDuration { get; set; }
    public decimal ServiceServiceCharge { get; set; }
    public required bool IsServiceActive { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}