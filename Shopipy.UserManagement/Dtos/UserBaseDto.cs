using System.ComponentModel.DataAnnotations;

namespace Shopipy.UserManagement.Dtos;

public class UserBaseDto
{
    [EmailAddress]
    public required string Email { get; set; }

    public required string Name { get; set; }
    
    public required UserRoleDto Role { get; set; }

    [Phone]
    public required string Phone { get; set; }

}