// =============================================================================
// RecoveryCodesResponseDto - Response for recovery codes retrieval
// =============================================================================

namespace Anima.Core.Dtos.Authentication;

/// <summary>
/// Response containing the list of active recovery codes for two-factor authentication (2FA).
/// </summary>
public class RecoveryCodesResponseDto
{
    /// <summary>
    /// The collection of generated recovery codes.
    /// </summary>
    public IEnumerable<string> Codes { get; set; } = Enumerable.Empty<string>();

    /// <summary>
    /// A descriptive message describing the result of the operation.
    /// </summary>
    public string Message { get; set; }
}

