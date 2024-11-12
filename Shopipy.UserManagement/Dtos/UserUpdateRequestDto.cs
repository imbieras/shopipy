namespace Shopipy.UserManagement.Dtos;

public class UserUpdateRequestDto : UserBaseDto
{
    public required UserStateDto UserState { get; set; }
}