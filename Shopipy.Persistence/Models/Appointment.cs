using System.ComponentModel.DataAnnotations;

namespace Shopipy.Persistence.Models;

public class Appointment
{
    [Key]
    public required int AppointmentId { get; set; }

    [StringLength(100)]
    public required string CustomerName { get; set; }

    [EmailAddress]
    public required string CustomerEmail { get; set; }

    [Phone]
    public required string CustomerPhone { get; set; }

    public required DateTime StartTime { get; set; }

    public required DateTime EndTime { get; set; }

    public required int EmployeeId { get; set; }

    public required int ServiceId { get; set; }
}