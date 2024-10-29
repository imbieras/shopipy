using Microsoft.AspNetCore.Mvc;

namespace Shopipy.Web.Controllers;

public class CounterController : Controller
{
    public IActionResult Index()
    {
        ViewData["Title"] = "Counter";
        return View();
    }
}