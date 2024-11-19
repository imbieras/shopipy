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
            // Skip middleware for swagger and Business controller endpoints
            if (context.Request.Path.StartsWithSegments("/swagger") || 
                context.Request.Path.StartsWithSegments("/Business"))
            {
                await _next(context);
                return;
            }

            var pathSegments = context.Request.Path.Value?
                .Split('/', StringSplitOptions.RemoveEmptyEntries);

            // Check if we have at least 2 segments (/{controller}/{businessId}/...)
            if (pathSegments != null && pathSegments.Length >= 2)
            {
                // Try to parse the second segment as businessId
                if (int.TryParse(pathSegments[1], out int businessId))
                {
                    var businessExists = await businessService.GetBusinessByIdAsync(businessId);
                    if (businessExists == null)
                    {
                        context.Response.StatusCode = StatusCodes.Status404NotFound;
                        await context.Response.WriteAsync($"Business with ID {businessId} not found.");
                        return;
                    }
                    
                    // Business exists, continue the pipeline
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