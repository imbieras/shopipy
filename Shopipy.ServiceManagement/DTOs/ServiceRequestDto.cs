using System.ComponentModel.DataAnnotations;

namespace Shopipy.ServiceManagement.DTOs;

public class ServiceRequestDto
{
    public required int CategoryId { get; set; }
    
    [StringLength(256)]
    public required string ServiceName { get; set; }
    
    [StringLength(256)]
    public required string ServiceDescription { get; set; }
    
    public required decimal ServiceBasePrice { get; set; }
    
    public required int ServiceDuration { get; set; }
    
    public decimal ServiceServiceCharge { get; set; }

    public required bool IsServiceActive { get; set; } = false;
}