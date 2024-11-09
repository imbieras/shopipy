using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopipy.ApiService.DTOs;
using Shopipy.UserManagement.Models;

namespace Shopipy.ApiService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(UserManager<User> userManager, SignInManager<User> signInManager)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user == null) return Unauthorized();
        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) return Unauthorized();
        var claimPrincipal = await signInManager.CreateUserPrincipalAsync(user);
        return SignIn(claimPrincipal);
    }

    [HttpGet("google")]
    public IActionResult GoogleSignIn()
    {
        return Challenge(GoogleDefaults.AuthenticationScheme);
    }
}