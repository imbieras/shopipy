using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Shopipy.ApiService.ExceptionFilters;

public class ArgumentExceptionFilter : IExceptionFilter
{
    public void OnException(ExceptionContext context)
    {
        if (context.Exception is not ArgumentException) return;
        context.Result = new BadRequestObjectResult(context.Exception.Message);
        context.ExceptionHandled = true;
    }
}