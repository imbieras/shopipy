using System.IdentityModel.Tokens.Jwt;

namespace Shopipy.Web.Helpers;

public class TokenHelper
{
    public static string? GetUserIdFromToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        if (!handler.CanReadToken(token)) return null;

        var tokenS = handler.ReadToken(token) as JwtSecurityToken;
        return tokenS?.Claims.FirstOrDefault(claim => claim.Type == JwtRegisteredClaimNames.Sub)?.Value;
    }
}