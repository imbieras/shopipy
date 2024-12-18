namespace Shopipy.ServiceManagement.DTOs;

public class ServiceResponseDto
{
    public required int BusinessId { get; init; }

    public required int CategoryId { get; init; }

    public required int ServiceId { get; init; }

    public required string ServiceName { get; init; }

    public required string ServiceDescription { get; init; }

    public required decimal ServicePrice { get; init; }

    public required int ServiceDuration { get; init; }

    public required bool IsServiceActive { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime UpdatedAt { get; init; }
}