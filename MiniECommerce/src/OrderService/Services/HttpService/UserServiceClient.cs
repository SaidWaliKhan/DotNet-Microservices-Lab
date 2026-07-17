using OrderService.Services.DTOs;

namespace OrderService.Services.HttpServices;

public class UserServiceClient
{
    private readonly HttpClient _httClient;

    public UserServiceClient(HttpClient httpClient)
    {
        _httClient = httpClient;
    }

    public async Task<UserDto?> GetUserAsync(int userId)
    {
        var user = await _httClient.GetAsync($"/api/users/{userId}");
        if (!user.IsSuccessStatusCode) return null;

        return await user.Content.ReadFromJsonAsync<UserDto>();
    }
}