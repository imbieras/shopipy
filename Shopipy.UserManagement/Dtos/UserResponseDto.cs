namespace Shopipy.UserManagement.Dtos;

public class UserResponseDto : UserBaseDto
{
    public required UserStateDto UserState { get; set; }
}