using Microsoft.AspNetCore.Identity;
using Shopipy.Persistence.Models;

namespace Shopipy.Shared.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor, UserManager<User> userManager)
{
    public async Task<User> GetCurrentUserAsync()
    {
        var httpContext = httpContextAccessor.HttpContext;
        if (httpContext == null) throw new InvalidOperationException("HttpContext is not available");
        var user = await userManager.GetUserAsync(httpContext.User);
        if (user == null) throw new InvalidOperationException("User is not found");
        return user;
    }
}