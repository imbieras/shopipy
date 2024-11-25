using Microsoft.AspNetCore.Mvc;
using Shopipy.Web.Services;
using Shopipy.UserManagement.Dtos;
using Shopipy.Web.ViewModels;

namespace Shopipy.Web.Controllers;

public abstract class BaseController(UserService userService) : Controller
{
    protected async Task<UserResponseDto?> GetCurrentUserAsync()
    {
        var userId = HttpContext.Items["UserId"]?.ToString();
        var token = HttpContext.Request.Cookies["BearerToken"];

        return await userService.GetCurrentUserAsync(userId, token);
    }

    public override ViewResult View(string? viewName, object? model)
    {
        if (model is BaseViewModel viewModel)
        {
            viewModel.CurrentUser = userService.GetCurrentUserAsync(
            HttpContext.Items["UserId"]?.ToString(),
            HttpContext.Request.Cookies["BearerToken"]
            ).Result;
        }

        return base.View(viewName, model);
    }
}