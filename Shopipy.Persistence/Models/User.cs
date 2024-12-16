using Microsoft.AspNetCore.Identity;

namespace Shopipy.Persistence.Models;

public class User : IdentityUser
{
    public User()
    {
    }

    public User(string userName) : base(userName)
    {
    }
    public required string Name { get; set; }
    public required UserRole Role { get; set; }
    public UserState UserState { get; set; } = UserState.Active;
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public int? BusinessId { get; set; }
    public Business? Business { get; set; }
}