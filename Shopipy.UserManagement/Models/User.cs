using Microsoft.AspNetCore.Identity;

namespace Shopipy.UserManagement.Models;

public class User : IdentityUser
{
    public User()
    {
    }

    public User(string userName) : base(userName)
    {
    }

    public required string Name { get; set; }
    public UserState UserState { get; set; } = UserState.Active;
}