namespace Shopipy.ServiceManagement.DTOs;

public class AppointmentResponseDto
{
    public required int BusinessId { get; init; }

    public required int AppointmentId { get; init; }

    public required string CustomerName { get; init; }

    public required string CustomerEmail { get; init; }

    public required string CustomerPhone { get; init; }

    public required DateTime StartTime { get; init; }

    public required DateTime EndTime { get; init; }

    public required Guid EmployeeId { get; init; }

    public required int ServiceId { get; init; }
}