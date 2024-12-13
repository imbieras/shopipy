using Microsoft.AspNetCore.Identity;
using Shopipy.Persistence.Models;
using Shopipy.Shared.Services;

namespace Shopipy.UserManagement.Services;

public class UserService(UserManager<User> userManager, CurrentUserService currentUserService, IBusinessService businessService)
{
    public async Task CreateUserAsync(User user, string password)
    {
        await ValidateUserAsync(user);
        var result = await userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            throw new ArgumentException(result.ToString());
        }
    }

    public async Task<IEnumerable<User>> GetAllUsersAsync(int? top = null, int? skip = null)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();
        var query = currentUser.Role == UserRole.SuperAdmin ? userManager.Users : userManager.Users.Where(user =>
            user.BusinessId == currentUser.BusinessId
        );
        query = query.OrderBy(user => user.Id).Skip(skip ?? 0);
        if (top is not null)
        {
            query = query.Take(top.Value);
        }
        
        return query;
    }

    public async Task<int> GetUserCountAsync()
    {
        var users = await GetAllUsersAsync();
        return users.Count();
    }

    public async Task<User?> GetUserByIdAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return null;
        try
        {
            await ValidateAccessToUser(user);
        } catch (UnauthorizedAccessException)
        {
            return null;
        }
        return user;
    }

    public async Task UpdateUserAsync(User user)
    {
        await ValidateUserAsync(user);
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            throw new ArgumentException(result.ToString());
        }
    }
    
    /// <returns>
    /// The <see cref="Task"/> that represents the asynchronous operation, containing the boolean that is true if user was deleted.
    /// </returns>
    public async Task<bool> DeleteUserAsync(string userId)
    {
        var user = await userManager.FindByIdAsync(userId);
        if (user == null) return false;
        try
        {
            await ValidateAccessToUser(user);
        }
        catch (UnauthorizedAccessException)
        {
            return false;
        }
        var result = await userManager.DeleteAsync(user);
        if (!result.Succeeded)
        {
            throw new ArgumentException(result.ToString());
        }
        return true;
    }
    
    private async Task ValidateUserAsync(User user)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();
        if (user.Role == UserRole.SuperAdmin && user.BusinessId != null) throw new ArgumentException("SuperAdmin cannot be part of business");
        if (user.Role != UserRole.SuperAdmin && user.BusinessId == null) throw new ArgumentException("Business must be provided");
        if (currentUser.Role == UserRole.BusinessOwner)
        {
            if (user.Role != UserRole.Employee)
            {
                throw new UnauthorizedAccessException("Business owner can only create employees");
            }
        }
        await ValidateAccessToUser(user);
        if (user.BusinessId != null)
        {
            var business = await businessService.GetBusinessByIdAsync(user.BusinessId.Value);
            if (business == null) throw new UnauthorizedAccessException("No access to business");
        }
    }

    private async Task ValidateAccessToUser(User user)
    {
        var currentUser = await currentUserService.GetCurrentUserAsync();
        if (currentUser.Role != UserRole.SuperAdmin && (user.BusinessId != currentUser.BusinessId ||
                                                        (currentUser.Role != UserRole.BusinessOwner && currentUser.Id != user.Id)))
        {
            throw new UnauthorizedAccessException(
                "No permission to use Business that current user is not the owner of");
        }
    }
}