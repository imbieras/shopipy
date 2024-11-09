namespace Shopipy.UserManagement.Dtos;

public class UserResponseDto : UserBaseDto
{
    public required string UserId { get; set; }
    public required UserStateDto UserState { get; set; }
    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}