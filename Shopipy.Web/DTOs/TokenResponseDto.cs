using System.Text.Json.Serialization;

namespace Shopipy.Web.DTOs;

public class TokenResponseDto
{
    [JsonPropertyName("tokenType")]
    public required string TokenType { get; init; }

    [JsonPropertyName("accessToken")]
    public required string AccessToken { get; init; }

    [JsonPropertyName("expiresIn")]
    public int ExpiresIn { get; init; }

    [JsonPropertyName("refreshToken")]
    public required string RefreshToken { get; init; }
}