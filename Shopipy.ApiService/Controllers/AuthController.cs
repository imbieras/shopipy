using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopipy.ApiService.DTOs;
using Shopipy.ApiService.Services;
using Shopipy.Persistence.Models;

namespace Shopipy.ApiService.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(UserManager<User> userManager, SignInManager<User> signInManager, AuthService authService)
    : ControllerBase
{
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginRequestDto request)
    {
        var user = await userManager.FindByNameAsync(request.UserName);
        if (user == null) return Unauthorized();
        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) return Unauthorized();
        var token = authService.GenerateToken(user);
        return Ok(new LoginResponseDto { Token = token });
    }
}