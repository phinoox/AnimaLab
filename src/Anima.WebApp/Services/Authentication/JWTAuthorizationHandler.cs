using Anima.WebApp.Services.Authentication;

public class JwtAuthorizationHandler : DelegatingHandler
{
    private readonly CustomAuthStateProvider _authStateProvider;
    private readonly UserTokenStore _userTokenStore;

    public JwtAuthorizationHandler(CustomAuthStateProvider authStateProvider,UserTokenStore userTokenStore)
    {
        _authStateProvider = authStateProvider;
        _userTokenStore = userTokenStore;
    }

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        // NO JS INTEROP HERE. We just read the C# property.
        var token = _userTokenStore.Token;
        
        if (!string.IsNullOrEmpty(token))
        {
            Console.WriteLine($"token requested from {_userTokenStore.GetHashCode()}:{_userTokenStore.Token}");
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        }

        return await base.SendAsync(request, cancellationToken);
    }
}