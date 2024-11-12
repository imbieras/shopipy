namespace Shopipy.UserManagement.Dtos;

public class UserRequestDto : UserBaseDto
{
    public required string Password { get; set; }
}