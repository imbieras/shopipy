using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Shopipy.UserManagement.Models;

namespace Shopipy.UserManagement.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(UserManager<User> userManager) : ControllerBase
{
    [HttpPost]
    public IActionResult Create()
    {
        return Ok();
    }
}