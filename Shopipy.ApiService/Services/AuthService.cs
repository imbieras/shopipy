using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using Shopipy.Persistence.Models;

namespace Shopipy.ApiService.Services;

public class AuthService(SigningCredentials signingCredentials, string issuer, string audience)
{
    public string GenerateToken(User user)
    {
        var handler = new JwtSecurityTokenHandler();
        var descriptor = new SecurityTokenDescriptor
        {
            SigningCredentials = signingCredentials,
            Expires = DateTime.UtcNow.AddHours(1),
            Issuer = issuer,
            Audience = audience,
            Claims = new Dictionary<string, object> { { JwtRegisteredClaimNames.Sub, user.Id }, { ClaimTypes.Role, user.Role.ToString() } }
        };
        return handler.CreateEncodedJwt(descriptor);
    }
}