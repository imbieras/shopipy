using Microsoft.AspNetCore.Http;
using Shopipy.BusinessManagement.Services;

namespace Shopipy.Persistence.Data.Middleware
{
    public class BusinessExistsMiddleware
    {
        private readonly RequestDelegate _next;

        public BusinessExistsMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, BusinessService businessService)
        {
            if (context.Request.Path.StartsWithSegments("/swagger") || 
                context.Request.Path.StartsWithSegments("/Business") ||
                context.Request.Path.StartsWithSegments("/Auth")) 
            {
                await _next(context);
                return;
            }

            var pathSegments = context.Request.Path.Value?
                .Split('/', StringSplitOptions.RemoveEmptyEntries);

            if (pathSegments != null && pathSegments.Length >= 2)
            {
                if (int.TryParse(pathSegments[1], out int businessId))
                {
                    var businessExists = await businessService.GetBusinessByIdAsync(businessId);
                    if (businessExists == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync($"Business with ID {businessId} not found.");
                        return;
                    }
                    
                    await _next(context);
                    return;
                }
            }

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsync("BusinessId is required in the URL path (/{controller}/{businessId}/...).");
            return;
        }
    }
}