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
public class UsersController(UserManager<User> userManager, IMapper mapper, UserService userService, ILogger<UsersController> logger) : ControllerBase
{
    [HttpGet]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<PaginationResultDto<UserResponseDto>>> GetUsers([FromQuery] int? top, [FromQuery] int? skip)
    {
        var users = await userService.GetAllUsersAsync(top, skip);
        var count = await userService.GetUserCountAsync();

        return Ok(new PaginationResultDto<UserResponseDto> { Data = users.Select(mapper.Map<UserResponseDto>), Count = count });
    }

    [HttpGet("{userId}")]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetUserById(string userId)
    {
        var user = await userService.GetUserByIdAsync(userId);
        if (user != null)
        {
            return Ok(mapper.Map<UserResponseDto>(user));
        }

        logger.LogWarning("User {UserId} not found.", userId);
        return NotFound();
    }
    
    [HttpPost]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] UserRequestDto request)
    {
        var user = mapper.Map<User>(request);
        await userService.CreateUserAsync(user, request.Password);
        return CreatedAtAction(nameof(GetUserById), new { userId = user.Id }, mapper.Map<UserResponseDto>(user));
    }

    [HttpPut("{userId}")]
    [Authorize(Policy = AuthorizationPolicies.RequireBusinessOwnerOrSuperAdmin)]
    public async Task<ActionResult<UserResponseDto>> Update(string userId, [FromBody] UserUpdateRequestDto request)
    {
        // Todo: password changing logic in auth management
        var user = await userManager.FindByIdAsync(userId);
        if (user == null)
        {
            logger.LogWarning("Attempted to update non-existent user {UserId}.", userId);
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
        if (deleted)
        {
            return NoContent();
        }

        logger.LogWarning("Attempted to delete non-existent user {UserId}.", userId);
        return NotFound();

    }
}