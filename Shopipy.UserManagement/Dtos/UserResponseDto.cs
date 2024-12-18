namespace Shopipy.UserManagement.Dtos;

public class UserResponseDto : UserBaseDto
{
    public required string UserId { get; init; }

    public required UserStateDto UserState { get; init; }

    public required DateTime CreatedAt { get; init; }

    public required DateTime UpdatedAt { get; init; }
}