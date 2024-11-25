using Microsoft.AspNetCore.Mvc;
using Shopipy.ApiService.DTOs;
using Shopipy.Web.DTOs;
using Shopipy.Web.Services;
using System.Text;
using System.Text.Json;

namespace Shopipy.Web.Controllers;

public class UserController(UserService userService, ILogger<UserController> logger, IHttpClientFactory httpClientFactory) : BaseController(userService)
{

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
            logger.LogWarning("Invalid login request received.");
            return View(loginRequest);
        }

        var client = httpClientFactory.CreateClient("Shopipy.ApiService");
        var response = await client.PostAsJsonAsync("/Auth/login", loginRequest);

        var responseContent = await response.Content.ReadAsStringAsync();

        if (!response.IsSuccessStatusCode)
        {
            logger.LogWarning("Login failed with status code {StatusCode}. Response: {Error}", response.StatusCode, responseContent);
            ModelState.AddModelError("", "Invalid username or password.");
            return View(loginRequest);
        }

        var tokenResponse = JsonSerializer.Deserialize<TokenResponseDto>(responseContent, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });

        if (tokenResponse != null)
        {
            // FIXME: The expiration time right now is hardcoded to 1 hour, since no refresh token is implemented.
            Response.Cookies.Append("BearerToken", tokenResponse.Token, new CookieOptions { HttpOnly = true, Secure = true, SameSite = SameSiteMode.Strict, Expires = DateTime.UtcNow.AddSeconds(3600) });
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