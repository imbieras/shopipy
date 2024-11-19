using System.ComponentModel.DataAnnotations;

namespace Shopipy.Persistence.Models;

public class Service
{
    public required int BusinessId { get; set; }
    
    public required int CategoryId { get; set; }
    
    [Key]
    public required int ServiceId { get; set; }
    
    [StringLength(255)]
    public required string ServiceName { get; set; }
    
    [StringLength(1000)]
    public required string ServiceDescription { get; set; }
    
    public required decimal ServiceBasePrice { get; set; }
    
    public required int ServiceDuration { get; set; }
    
    public decimal ServiceServiceCharge { get; set; }
    
    public required bool IsServiceActive { get; set; }
    
    public required DateTime CreatedAt { get; set; }
    
    public required DateTime UpdatedAt { get; set; }
}