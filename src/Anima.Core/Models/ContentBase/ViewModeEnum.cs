// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Anima.Core.Models.ContentBase;

/// <summary>
/// Defines the viewing mode for content items, distinguishing between administrative and public views.
/// </summary>
public enum ViewModeEnum
{
    /// <summary>The private writing/editing view, intended for administrators and creators.</summary>
    PrivateWriting = 0,
    
    /// <summary>The presentation view, intended for public viewing or sharing.</summary>
    Presentation = 1
}