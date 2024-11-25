using Shopipy.UserManagement.Dtos;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Shopipy.Web.Services;

public class UserService(IHttpClientFactory httpClientFactory)
{
    private readonly HttpClient _httpClient = httpClientFactory.CreateClient("Shopipy.ApiService");

    private UserResponseDto? _currentUser;
    public async Task<UserResponseDto?> GetCurrentUserAsync(string? userId, string? token)
    {
        if (_currentUser != null) return _currentUser;

        if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(token)) return null;

        try
        {
            _currentUser = await _httpClient.GetFromJsonAsync<UserResponseDto>($"/Users/{userId}", new JsonSerializerOptions { PropertyNameCaseInsensitive = true, Converters = { new JsonStringEnumConverter() } });
        }
        catch (Exception)
        {
            _currentUser = null;
        }

        return _currentUser;
    }
}