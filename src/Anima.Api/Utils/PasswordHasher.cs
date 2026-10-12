namespace Anima.Api.Utils;

/// <summary>
/// Provides utility methods for hashing and verifying passwords using BCrypt.
/// </summary>
public static class PasswordHasher
{
    /// <summary>
    /// Hashes a plain-text password.
    /// </summary>
    /// <param name="password">The plain-text password.</param>
    /// <returns>A hashed password string.</returns>
    public static string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty.", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verifies a plain-text password against a stored hash.
    /// </summary>
    /// <param name="password">The plain-text password.</param>
    /// <param name="hashedPassword">The stored hash.</param>
    /// <returns>True if the password matches the hash; otherwise, false.</returns>
    public static bool Verify(string password, string hashedPassword)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hashedPassword))
            return false;

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hashedPassword);
        }
        catch
        {
            // Handle potential exceptions from the underlying library (e.s. malformed hash)
            return false;
        }
    }
}
