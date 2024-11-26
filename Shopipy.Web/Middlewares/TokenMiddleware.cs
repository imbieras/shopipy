using Shopipy.Web.Helpers;

namespace Shopipy.Web.Middlewares;

public class TokenMiddleware(RequestDelegate next)
{

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Cookies["BearerToken"];
        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers.Authorization = $"Bearer {token}";

            var userId = TokenHelper.GetUserIdFromToken(token);

            if (!string.IsNullOrEmpty(userId))
            {
                context.Items["UserId"] = userId;
            }
        }

        await next(context);
    }
}