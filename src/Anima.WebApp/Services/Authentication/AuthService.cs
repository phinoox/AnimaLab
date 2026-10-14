using Anima.Core.Dtos.Authentication;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Authentication;

namespace Anima.WebApp.Services.Authentication;

public class AuthService
{
    private readonly HttpClient _http;

    public AuthService(HttpClient http) => _http = http;

    public async Task<ApiResponseDto<AuthResponseDto>?> SignIn(SignInDto signInDto)
    {
        var response = await _http.PostAsJsonAsync("api/v1/auth/signin", signInDto);
        //Console.WriteLine(await response.Content.ReadAsStringAsync());
        var responseDto = await response.Content.ReadFromJsonAsync<ApiResponseDto<AuthResponseDto>>();
        return responseDto;
    }

    public async Task<ApiResponseDto<MetaIdentityResponseDto>> Register(RegisterDto registerDto)
    {
        var response = await _http.PostAsJsonAsync("api/v1/auth/register", registerDto);
        var content = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"got repsonse:{content}");
        var responseDto = await response.Content.ReadFromJsonAsync<ApiResponseDto<MetaIdentityResponseDto>>();
        return responseDto;
    }

    public async Task<ApiResponseDto<UserMetaInfo>> GetUserProfile()
    {
        return await _http.GetFromJsonAsync<ApiResponseDto<UserMetaInfo>>("api/v1/users/profile");
    }

    public async Task<string> GetUserProfileString()
    {
        var response = await _http.GetAsync("api/v1/users/profile");
        var content = await response.Content.ReadAsStringAsync();
        var message = $"{response.StatusCode}: {content}";
        return message;
    }
}