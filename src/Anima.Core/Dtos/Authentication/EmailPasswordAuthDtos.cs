namespace Anima.Core.Dtos.Authentication;

using System.ComponentModel.DataAnnotations;

/// <summary>
/// Data required to register a new user via email/password.
/// </summary>
public class RegisterDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required, MinLength(8)]
    public string Password { get; set; } = "";

    [Required]
    public string UserName { get; set; } = "";
}

/// <summary>
/// Data required to sign in a user via email/password.
/// </summary>
public class SignInDto
{
    [Required, EmailAddress]
    public string Email { get; set; } = "";

    [Required]
    public string Password { get; set; } = "";
}

/// <summary>
/// Represents the identity information of a user returned after successful authentication.
/// </summary>
public class UserResponseDto
{
    /// <summary>
    /// The unique identifier for the user.
    /// </summary>
    public Guid Id { get; set; } 

    /// <summary>
    /// The full name of the user.
    /// </summary>
    public string UserName { get; set; } = "";

    /// <summary>
    /// The primary email address associated with the account.
    /// </summary>
    public string Email { get; set; } = "";
    public DateTime CreatedAt { get; set; }
}

/// <summary>
/// A unified response containing security tokens and user context for all authentication methods.
/// </summary>
public class AuthResponseDto
{
    /// <summary>
    /// The JWT bearer token or session identifier used for authenticated requests.
    /// </summary>
    public string AccessToken { get; set; } = "";

    /// <summary>
    /// The type of the token (e.g., "Bearer").
    /// </summary>
    public string TokenType { get; set; } = "";

    /// <summary>
    /// The time-to-live for the token in seconds.
    /// </summary>
    public int ExpiresInSeconds { get; set; }

    /// <summary>
    /// An optional human-readable message providing context success or failure.
    /// </summary>
    public string? Message { get; set; }

    /// <summary>
    /// The user identity information associated with this authentication session.
    /// </summary>
    public UserResponseDto User { get; set; } = null!;
}
