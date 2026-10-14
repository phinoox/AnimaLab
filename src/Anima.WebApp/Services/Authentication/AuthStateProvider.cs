using System.Security.Claims;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components.Authorization;

namespace Anima.WebApp.Services.Authentication;

public class CustomAuthStateProvider : AuthenticationStateProvider
{
    private readonly ILocalStorageService _localStorage;
    private readonly UserTokenStore _tokenStore;
    private readonly ClaimsPrincipal _anonymous = new ClaimsPrincipal(new ClaimsIdentity());

    private bool _initialized = false;
    public string? Token {get;set;}

    public UserTokenStore TokenStore => _tokenStore;

    public CustomAuthStateProvider(ILocalStorageService localStorage, UserTokenStore tokenStore)
    {
        _localStorage = localStorage;
        _tokenStore = tokenStore;
    }

    // 1. The Central Sync Method called from OnAfterRenderAsync
    public async Task InitializeAsync()
    {
        if(_initialized)
            return;
        try
        {
            if(string.IsNullOrEmpty(_tokenStore.Token))
                _tokenStore.Token = await _localStorage.GetItemAsync<string>("authToken");

            var token = _tokenStore.Token;
            if (!string.IsNullOrWhiteSpace(token))
            {
                var identity = ParseClaimsFromJwt(token);
                NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
                _initialized = true;
            }
            Console.WriteLine($"token written to {TokenStore.GetHashCode()}: {token}");
        }
        catch { Console.WriteLine($"could not write token to {TokenStore.GetHashCode()}");}
    }

    // 2. Called by components after successful Login
    public async Task SetUserAsync(string token)
    {
        _tokenStore.Token = Token = token;
        await _localStorage.SetItemAsync("authToken", token);
        
        var identity = ParseClaimsFromJwt(token);
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(new ClaimsPrincipal(identity))));
    }

    // 3. Called by components for Logout
    public async Task ClearUserAsync()
    {
        _tokenStore.Token = Token = null;
        await _localStorage.RemoveItemAsync("authToken");
        NotifyAuthenticationStateChanged(Task.FromResult(new AuthenticationState(_anonymous)));
    }

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        // This is still used by Blazor internally; 
        // we rely on InitializeAsync to populate the memory store.
        return new AuthenticationState(_anonymous); 
    }

    private ClaimsIdentity ParseClaimsFromJwt(string jwt)
    {
        var handler = new System.IdentityModel.Tokens.Jwt.JwtSecurityTokenHandler();
        var jsonToken = handler.ReadJwtToken(jwt);
        return new ClaimsIdentity(jsonToken.Claims.Select(c => new Claim(c.Type, c.Value)), "jwt");
    }
}