using Microsoft.AspNetCore.Identity;

namespace Shopipy.ApiService.Models;

public class User(string userName) : IdentityUser(userName);