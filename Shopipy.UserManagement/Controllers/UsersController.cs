using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopipy.Persistence.Models;
using Shopipy.Shared;
using Shopipy.Shared.DTOs;
using Shopipy.UserManagement.Dtos;
using Shopipy.UserManagement.Services;

namespace Shopipy.UserManagement.Controllers;

[ApiController]
[Route("users")]
[Authorize]
public class UsersController(UserManager<User> userManager, IMapper mapper, UserService userService) : ControllerBase
{
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] UserRequestDto request)
    {
        var user = mapper.Map<User>(request);
        await userService.CreateUserAsync(user, request.Password);
        return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, mapper.Map<UserResponseDto>(user));
    }

    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<PaginationResultDto<UserResponseDto>>> GetUsers([FromQuery] int? top, [FromQuery] int? skip)
    {
        var users = await userService.GetAllUsersAsync(top, skip);
        var count = await userService.GetUserCountAsync();
        
        return Ok(new PaginationResultDto<UserResponseDto>
        {
            Data = users.Select(mapper.Map<UserResponseDto>),
            Count = count
        });
    }
    
    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUserById(string userId)
    {
        var user = await userService.GetUserByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        return Ok(mapper.Map<UserResponseDto>(user));
    }

    [HttpPut("{userId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<UserResponseDto>> Update(string userId, [FromBody] UserUpdateRequestDto request)
    {
        // Todo: password changing logic in auth management
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound();
        }
        mapper.Map(request, user);
        await userService.UpdateUserAsync(user);
        return Ok(mapper.Map<UserResponseDto>(user));
    }
    
    [HttpDelete("{userId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> Delete(string userId)
    {
        // Todo: authorization check
        var deleted = await userService.DeleteUserAsync(userId);
        return deleted ? NoContent() : NotFound();
    }
}