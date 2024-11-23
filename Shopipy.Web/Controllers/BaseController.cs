using Microsoft.AspNetCore.Mvc;
using Shopipy.Web.Services;
using Shopipy.UserManagement.Dtos;
using Shopipy.Web.ViewModels;

namespace Shopipy.Web.Controllers;

public abstract class BaseController(UserService userService) : Controller
{

    private UserResponseDto? _currentUser;

    protected async Task<UserResponseDto?> GetCurrentUserAsync()
    {
        if (_currentUser != null) return _currentUser;

        var userId = HttpContext.Items["UserId"]?.ToString();
        var token = HttpContext.Request.Cookies["BearerToken"];

        if (!string.IsNullOrEmpty(userId) && !string.IsNullOrEmpty(token))
        {
            _currentUser = await userService.GetUserByIdAsync(userId, token);
        }

        return _currentUser;
    }

    public override ViewResult View(string? viewName, object? model)
    {
        if (model is BaseViewModel viewModel)
        {
            viewModel.CurrentUser = _currentUser;
        }

        return base.View(viewName, model);
    }
}