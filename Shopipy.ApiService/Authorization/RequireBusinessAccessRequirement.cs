using Microsoft.AspNetCore.Authorization;
using Shopipy.Persistence.Models;
using Shopipy.Shared.Services;

namespace Shopipy.ApiService.Authorization;

public class RequireBusinessAccessRequirement : IAuthorizationRequirement
{
}

public class RequireBusinessAccessRequirementHandler : AuthorizationHandler<RequireBusinessAccessRequirement>
{
    protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, RequireBusinessAccessRequirement requirement)
    {
        var httpContext = context.Resource as HttpContext;
        if (httpContext is null)
        {
            throw new InvalidOperationException("HttpContext unavailable");
        }
        var businessId = int.Parse(httpContext.GetRouteValue("businessId")!.ToString()!);
        var currentUserService = httpContext.RequestServices.GetRequiredService<CurrentUserService>();
        var user = await currentUserService.GetCurrentUserAsync();
        if (user.Role != UserRole.SuperAdmin && user.BusinessId != businessId)
        {
            context.Fail();
            return;
        }
        context.Succeed(requirement);
    }
}