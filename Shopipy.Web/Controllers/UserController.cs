using Microsoft.AspNetCore.Mvc;
using Shopipy.ApiService.DTOs;
using Shopipy.Web.DTOs;
using System.Text;
using System.Text.Json;

namespace Shopipy.Web.Controllers;

public class UserController(ILogger<UserController> logger, IHttpClientFactory httpClientFactory) : Controller
{
    private readonly ILogger<UserController> _logger = logger;

    [HttpGet]
    public IActionResult Login()
    {
        if (User.Identity?.IsAuthenticated == true)
        {
            return RedirectToAction("Index", "Home");
        }

        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Login(LoginRequestDto loginRequest)
    {
        if (!ModelState.IsValid)
        {
            return View(loginRequest);
        }

        var client = httpClientFactory.CreateClient("Shopipy.ApiService");
        var requestContent = new StringContent(JsonSerializer.Serialize(loginRequest), Encoding.UTF8, "application/json");

        var response = await client.PostAsync("/Auth/login", requestContent);

        if (!response.IsSuccessStatusCode)
        {
            ModelState.AddModelError("", "Invalid username or password.");
            return View(loginRequest);
        }

        var responseContent = await response.Content.ReadAsStringAsync();

        Console.WriteLine(responseContent);

        var tokenResponse = JsonSerializer.Deserialize<TokenResponseDto>(responseContent);

        if (tokenResponse != null)
        {
            Response.Cookies.Append("BearerToken", tokenResponse.AccessToken, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddSeconds(tokenResponse.ExpiresIn) });
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpPost]
    public IActionResult Logout()
    {
        Response.Cookies.Delete("BearerToken");
        return RedirectToAction("Index", "Home");
    }
}