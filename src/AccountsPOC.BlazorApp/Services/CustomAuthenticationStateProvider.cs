using System.Net.Http.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Components.Authorization;

namespace AccountsPOC.BlazorApp.Services;

public class CustomAuthenticationStateProvider : AuthenticationStateProvider
{
    private readonly HttpClient _httpClient;
    private ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    public CustomAuthenticationStateProvider(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        try
        {
            var token = await GetTokenAsync();
            if (string.IsNullOrEmpty(token))
            {
                return new AuthenticationState(_anonymous);
            }

            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

            // Use a timeout to prevent hanging if API is not available
            using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(3));
            var response = await _httpClient.GetAsync("api/auth/me", cts.Token);
            
            if (!response.IsSuccessStatusCode)
            {
                await ClearTokenAsync();
                return new AuthenticationState(_anonymous);
            }

            var userInfo = await response.Content.ReadFromJsonAsync<UserInfoDto>(cancellationToken: cts.Token);
            if (userInfo == null)
            {
                await ClearTokenAsync();
                return new AuthenticationState(_anonymous);
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, userInfo.UserId.ToString()),
                new Claim(ClaimTypes.Name, userInfo.Username),
                new Claim(ClaimTypes.Email, userInfo.Email),
                new Claim("tenant_id", userInfo.TenantId.ToString())
            };

            if (!string.IsNullOrEmpty(userInfo.FirstName))
                claims.Add(new Claim(ClaimTypes.GivenName, userInfo.FirstName));
            
            if (!string.IsNullOrEmpty(userInfo.LastName))
                claims.Add(new Claim(ClaimTypes.Surname, userInfo.LastName));

            foreach (var role in userInfo.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var identity = new ClaimsIdentity(claims, "jwt");
            var user = new ClaimsPrincipal(identity);

            return new AuthenticationState(user);
        }
        catch (Exception ex)
        {
            // Log the error silently but don't throw - return anonymous user
            Console.WriteLine($"Authentication check failed: {ex.Message}");
            await ClearTokenAsync();
            return new AuthenticationState(_anonymous);
        }
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        try
        {
            var loginDto = new { email, password };
            var response = await _httpClient.PostAsJsonAsync("api/auth/login", loginDto);
            
            if (!response.IsSuccessStatusCode)
            {
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                return false;
            }

            await SaveTokenAsync(result.Token);
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<bool> RegisterAsync(RegisterDto dto)
    {
        try
        {
            var response = await _httpClient.PostAsJsonAsync("api/auth/register", dto);
            
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"Registration failed: {error}");
                return false;
            }

            var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            if (result == null || string.IsNullOrEmpty(result.Token))
            {
                return false;
            }

            await SaveTokenAsync(result.Token);
            _httpClient.DefaultRequestHeaders.Authorization = 
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", result.Token);

            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Registration exception: {ex.Message}");
            return false;
        }
    }

    public async Task LogoutAsync()
    {
        await ClearTokenAsync();
        _httpClient.DefaultRequestHeaders.Authorization = null;
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    private async Task<string?> GetTokenAsync()
    {
        return await Task.FromResult(_token);
    }

    private async Task SaveTokenAsync(string token)
    {
        _token = token;
        await Task.CompletedTask;
    }

    private async Task ClearTokenAsync()
    {
        _token = null;
        await Task.CompletedTask;
    }

    private string? _token;
}

public class UserInfoDto
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public int TenantId { get; set; }
    public bool IsActive { get; set; }
    public List<string> Roles { get; set; } = new();
}

public class AuthResponseDto
{
    public required string Token { get; set; }
    public required string Email { get; set; }
    public required string Username { get; set; }
    public int UserId { get; set; }
    public int TenantId { get; set; }
}

public class RegisterDto
{
    public int TenantId { get; set; }
    public required string Username { get; set; }
    public required string Email { get; set; }
    public required string Password { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? PhoneNumber { get; set; }
    public List<string>? RoleNames { get; set; }
}
