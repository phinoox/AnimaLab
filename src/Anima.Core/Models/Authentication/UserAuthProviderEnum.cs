namespace Anima.Core.Models.Authentication;

/// <summary>
/// Specifies the authentication provider used for user login.
/// </summary>
public enum UserAuthProviderEnum
{
    /// <summary>
    /// Standard email and password-based authentication (e.g., using PBKDF2).
    /// May be augmented with Time-based One-Time Password (TOTP) two-factor authentication.
    /// </summary>
    Password = 0,

    /// <summary>
    /// Authentication via Google OAuth using the identity token's 'sub' claim.
    /// </summary>
    Google = 1
}