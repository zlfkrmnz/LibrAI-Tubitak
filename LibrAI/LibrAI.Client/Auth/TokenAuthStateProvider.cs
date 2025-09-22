using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

public class TokenAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _storage;
    private readonly AuthClient _auth;

    public TokenAuthStateProvider(ILocalStorageService storage, AuthClient auth)
    {
        _storage = storage; _auth = auth;
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        var token = await _storage.GetItemAsync<string>("access_token");
        var user = _auth.GetPrincipalOrAnonymous(token);
        return new AuthenticationState(user);
    }

    public async Task MarkUserAsAuthenticated(string token)
    {
        await _storage.SetItemAsync("access_token", token);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }

    public async Task MarkUserAsLoggedOut()
    {
        await _storage.RemoveItemAsync("access_token");
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}
