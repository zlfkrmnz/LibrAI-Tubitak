using Blazored.LocalStorage;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Security.Claims;

public class AuthClient
{
    private readonly HttpClient _http;
    private readonly ILocalStorageService? _storage; // <-- opsiyonel

    public AuthClient(HttpClient http, ILocalStorageService? storage = null) // <-- opsiyonel ctor
    {
        _http = http;
        _storage = storage;
    }

    public async Task<bool> RegisterAsync(string email, string password, string? fullName = null)
    {
        var res = await _http.PostAsJsonAsync("/register?useCookies=false", new { email, password });
        if (!res.IsSuccessStatusCode) return false;
        return true;
    }

    public async Task<bool> LoginAsync(string email, string password)
    {
        var res = await _http.PostAsJsonAsync("/login?useCookies=false&useSessionCookies=false",
            new { email, password });

        if (!res.IsSuccessStatusCode) return false;

        var token = await res.Content.ReadFromJsonAsync<TokenResponse>();
        if (string.IsNullOrWhiteSpace(token?.AccessToken)) return false;

        // Header her iki tarafta da ayarlanabilir
        _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.AccessToken);

        // localStorage sadece browser’da
        if (OperatingSystem.IsBrowser() && _storage is not null)
        {
            await _storage.SetItemAsync("access_token", token!.AccessToken);
        }

        return true;
    }

    public async Task LogoutAsync()
    {
        _http.DefaultRequestHeaders.Authorization = null;

        if (OperatingSystem.IsBrowser() && _storage is not null)
        {
            await _storage.RemoveItemAsync("access_token");
        }
        await Task.CompletedTask;
    }

    public async Task RestoreAsync()
    {
        if (OperatingSystem.IsBrowser() && _storage is not null)
        {
            var token = await _storage.GetItemAsync<string>("access_token");
            if (!string.IsNullOrWhiteSpace(token))
                _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        }
        // Server prerender’da hiçbir şey yapma (cookie ile auth kullanıyorsan burada ayrı logic kurabilirsin)
    }

    public ClaimsPrincipal GetPrincipalOrAnonymous(string? token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return new ClaimsPrincipal(new ClaimsIdentity());

        var jwt = new JwtSecurityTokenHandler().ReadJwtToken(token);
        var id = new ClaimsIdentity(jwt.Claims, authenticationType: "Bearer");
        return new ClaimsPrincipal(id);
    }

    private sealed record TokenResponse(string? AccessToken, string? TokenType, long? ExpiresIn);
}
