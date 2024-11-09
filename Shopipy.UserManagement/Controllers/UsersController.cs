using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopipy.UserManagement.Dtos;
using Shopipy.UserManagement.Models;

namespace Shopipy.UserManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(UserManager<User> userManager, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] UserRequestDto request)
    {
        var user = mapper.Map<User>(request);
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        return Created("", mapper.Map<UserResponseDto>(user));
    }
}