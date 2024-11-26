using System.Text.Json.Serialization;

namespace Shopipy.Web.DTOs;

public class TokenResponseDto
{
    [JsonPropertyName("token")]
    public required string Token { get; init; }
}