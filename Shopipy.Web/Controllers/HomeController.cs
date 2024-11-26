using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Shopipy.Web.Models;
using Shopipy.Web.Services;
using Shopipy.Web.ViewModels;

namespace Shopipy.Web.Controllers;

public class HomeController(UserService userService, ILogger<HomeController> logger) : BaseController(userService)
{
    public async Task<IActionResult> Index()
    {
        var currentUser = await GetCurrentUserAsync();

        if (currentUser == null)
        {
            return RedirectToAction("Login", "User");
        }

        ViewData["Title"] = "Home";
        ViewData["UserName"] = currentUser.Name;

        var viewModel = new HomeViewModel { CurrentUser = currentUser };

        return View(viewModel);
    }


    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}