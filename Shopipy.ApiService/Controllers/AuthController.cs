using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopipy.ApiService.DTOs;

namespace Shopipy.ApiService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(UserManager<IdentityUser> userManager, SignInManager<IdentityUser> signInManager)
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
}