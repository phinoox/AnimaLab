namespace Anima.Core.Models.Shared;

/// <summary>
/// Specifies the type of activity performed within the system for auditing purposes.
/// </summary>
public enum ActivityTypeEnum
{
    /// <summary>Indicates a creation event.</summary>
    Create = 0,

    /// <summary>Indicates an update event.</summary>
    Update = 1,

    /// <summary>Indicates a deletion event.</summary>
    Delete = 2,

    /// <summary>Indicates a login event.</summary>
    Login = 3,

    /// <summary>Indicates a logout event.</summary>
    Logout = 4,

    /// <summary>Indicates token usage activity.</summary>
    TokenUsage = 5
}