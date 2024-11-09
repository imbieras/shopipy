using Microsoft.AspNetCore.Identity;

namespace Shopipy.UserManagement.Models;

public class User(string userName) : IdentityUser(userName);