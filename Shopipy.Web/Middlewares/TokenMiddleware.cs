namespace Shopipy.Web.Middlewares;

public class TokenMiddleware(RequestDelegate next)
{

    public async Task Invoke(HttpContext context)
    {
        var token = context.Request.Cookies["BearerToken"];
        if (!string.IsNullOrEmpty(token))
        {
            context.Request.Headers.Authorization = $"Bearer {token}";
        }

        await next(context);
    }
}