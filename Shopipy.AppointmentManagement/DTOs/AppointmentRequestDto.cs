using System.ComponentModel.DataAnnotations;

namespace AppointmentManagement.DTOs;

public class AppointmentRequestDto
{
  public required int EmployeeId { get; set; }
  public required int ServiceId { get; set; }

  [StringLength(100, ErrorMessage = "Customer name must not exceed 100 characters.")]
  public required string CustomerName { get; set; }
  
  [StringLength(255)]
  [EmailAddress(ErrorMessage = "Invalid email format.")]
  public required string CustomerEmail { get; set; }

  [Phone(ErrorMessage = "Invalid phone number format.")]
  public required string CustomerPhone { get; set; }
  
  public required DateTime StartTime { get; set; }
  public bool SendSmsNotification { get; set; } = false;
}
