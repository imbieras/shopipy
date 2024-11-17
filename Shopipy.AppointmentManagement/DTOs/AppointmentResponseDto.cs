namespace AppointmentManagement.DTOs;

public class AppointmentResponseDto
{
  public required int AppointmentId { get; set; }
  public required string CustomerName { get; set; }
  public required string CustomerEmail { get; set; }
  public required string CustomerPhone { get; set; }
  public required DateTime StartTime { get; set; }
  public required DateTime EndTime { get; set; }
  public required int EmployeeId { get; set; }
  public required int ServiceId { get; set; }
}