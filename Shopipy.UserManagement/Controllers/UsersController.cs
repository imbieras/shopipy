using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopipy.Persistance.Models;
using Shopipy.UserManagement.Dtos;

namespace Shopipy.UserManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(UserManager<User> userManager, IMapper mapper) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] UserRequestDto request)
    {
        // Todo: perform role authorized checking
        var user = mapper.Map<User>(request);
        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        return Created("", mapper.Map<UserResponseDto>(user));
    }

    [HttpGet]
    public ActionResult<IEnumerable<UserResponseDto>> Get()
    {
        // Todo: return subset of users depending on role
        return Ok(userManager.Users.Select(u => mapper.Map<UserResponseDto>(u)));
    }
    
    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> Get(string userId)
    {
        // Todo: authorization check
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<UserResponseDto>(user));
    }

    [HttpPut("{userId}")]
    public async Task<ActionResult<UserResponseDto>> Update(string userId, [FromBody] UserUpdateRequestDto request)
    {
        // Todo: auth check
        // Todo: password changing logic in auth management
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        mapper.Map(request, user);
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            return BadRequest(result.Errors);
        }
        return Ok(mapper.Map<UserResponseDto>(user));
    }
    
    [HttpDelete("{userId}")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> Delete(string userId)
    {
        // Todo: authorization check
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }

        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, result.Errors);
        }
        return NoContent();
    }
}