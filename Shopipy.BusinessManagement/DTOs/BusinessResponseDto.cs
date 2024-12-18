using Shopipy.Persistence.Models;

namespace Shopipy.BusinessManagement.DTOs;

public class BusinessResponseDto
{
    public int BusinessId { get; init; }

    public required string Name { get; init; }

    public required string Address { get; init; }

    public string? Phone { get; init; }

    public required string Email { get; init; }

    public string? VatNumber { get; init; }

    public required BusinessType BusinessType { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime UpdatedAt { get; init; }
}