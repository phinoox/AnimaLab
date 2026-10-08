using Anima.Core.Models.Authentication;

namespace Anima.Api.Extensions;

/// <summary>
/// Represents the identity and authorization context of the currently authenticated user within a request scope.
/// </summary>
[ServiceLifetime(ServiceLifetime.Scoped)]
public class UserContext : IUserContext
{
    private readonly ILogger<UserContext> _logger;

    /// <summary>
    /// Initializes a new instance of the <see cref="UserContext"/> class.
    /// </summary>
    /// <param name="logger">The logger for recording context-related events and errors.</param>
    public UserContext(ILogger<UserContext> logger) 
    {
        _logger = logger;
    }

    /// <summary>
    /// Gets or sets the unique identifier of the current user.
    /// </summary>
    public Guid? UserId { get; set; }

    /// <summary>
    /// Gets or sets the full user entity object for the current context.
    /// </summary>
    public User? CurrentUser { get; set; }

    /// <summary>
    /// Gets or sets the list of roles assigned to the current user.
    /// </summary>
    public List<string>? Roles { get; set; }

    /// <summary>
    /// Gets or sets the hashed API token associated with the current session.
    /// </summary>
    public string? ApiTokenHash {get;set;}

    /// <summary>
    /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
    /// </summary>
    public void Dispose()
    {
        // Implementation for resource cleanup if required in the future.
    }
}