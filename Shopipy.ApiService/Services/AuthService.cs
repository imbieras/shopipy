using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Shopipy.Persistence.Models;

namespace Shopipy.ApiService.Services;

public class AuthService(IConfiguration config)
{
    public string GenerateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["Authentication:Jwt:Key"]!));
        var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        
        var descriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = signingCredentials,
            Expires = DateTime.UtcNow.AddHours(1),
            Claims = new Dictionary<string, object>
            {
                {JwtRegisteredClaimNames.Sub, user.Id}
            }
        };
        return handler.CreateEncodedJwt(descriptor);
    }
}