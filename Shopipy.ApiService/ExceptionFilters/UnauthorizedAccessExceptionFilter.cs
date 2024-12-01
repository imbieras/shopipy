using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shopipy.ApiService.ExceptionFilters;

public class UnauthorizedAccessExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not UnauthorizedAccessException) return;
        context.Result = new UnauthorizedResult();
        context.ExceptionHandled = true;
    }
}