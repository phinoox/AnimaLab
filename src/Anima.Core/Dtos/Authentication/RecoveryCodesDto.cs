// =============================================================================
// =============================================================================

namespace Anima.Core.Dtos.Authentication;

/// <summary>
/// Data transfer object for requesting and viewing user recovery codes.
/// </summary>
public class RecoveryCodesDto
{
    /// <summary>
    /// The preferred format of the recovery codes (e.g., "csv" or "text").
    /// </summary>
    [MaxLength(16)]
    public string? Format { get; set; } = "csv";

    /// <summary>
    /// The unique identifier of the user these codes belong to.
    /// </summary>
    public Guid? UserId { get; set; }
}