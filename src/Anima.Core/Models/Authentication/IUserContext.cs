namespace Anima.Core.Models.Authentication;

/// <summary>
/// Provides information about the currently authenticated user within the system context.
/// </summary>
public interface IUserContext : IDisposable
{
    /// <summary>
    /// Gets or sets the current authenticated user, if any.
    /// </summary>
    User? CurrentUser { get; set; }
    
    /// <summary>
    /// Gets or sets the unique identifier of the current user, or null if not authenticated.
    /// </summary>
    Guid? UserId { get; set; }
    
    /// <summary>
    /// Gets or sets the hash of the API token used for authentication, if applicable.
    /// </summary>
    string? ApiTokenHash { get;set; }
    
    /// <summary>
    /// Gets or sets the list of roles assigned to the current user for authorization checks.
    /// </summary>
    List<string> Roles { get; set;}
}
