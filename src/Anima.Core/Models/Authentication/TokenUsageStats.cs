namespace Anima.Core.Models.Authentication;

/// <summary>
/// Provides statistics regarding the usage and remaining quota of a project token.
/// </summary>
public class TokenUsageStats
{
    /// <summary>
    /// Gets or sets the total number of requests allowed by the token.
    /// </summary>
    public int TotalUsage { get; set; }

    /// <summary>
    /// Gets or sets the remaining number of requests available for this token.
    /// </summary>
    public int RemainingUsage { get; set; }

    /// <summary>
    /// Gets or sets the expiration timestamp as an object.
    /// </summary>
    public object ExpiresAt { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the token has expired.
    /// </summary>
    public bool IsExpired { get; set; }
}
