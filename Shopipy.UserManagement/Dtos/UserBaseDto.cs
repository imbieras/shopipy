using System.ComponentModel.DataAnnotations;

namespace Shopipy.UserManagement.Dtos;

public class UserBaseDto
{
    [EmailAddress]
    public required string Email { get; init; }

    public required string Name { get; init; }

    public required UserRoleDto Role { get; init; }

    [Phone]
    public required string Phone { get; init; }

    public int? BusinessId { get; init; }
}