namespace Anima.Api.Extensions;

[ServiceLifetime(ServiceLifetime.Scoped)]
public class UserContext : IUserContext
{
    private readonly ILogger<UserContext> _logger;
    public UserContext(ILogger<UserContext> logger) 
    {
        _logger = logger;
    }

    // Properties as defined in your doc
    public Guid? UserId { get; set; }
    public User? CurrentUser { get; set; }
    public List<string>? Roles { get; set; }

    public string? ApiTokenHash {get;set;}

    // Constructor must match CoreService requirements 
    // (passing logger and potentially userContext/other dependencies)
    
    public void Dispose()
    {
        
    }
}