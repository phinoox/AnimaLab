// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Anima.Core.Models.Projects;

/// <summary>
/// Defines the primary format of a project, which dictates default UI layouts and workspace focus.
/// </summary>
public enum PrimaryFormatEnum
{
    /// <summary>Standard text-based book or novel layout.</summary>
    [Display(Name = "Book")]
    Book = 0,
    
    /// <summary>Panel-based layout optimized for manga or comic formats.</summary>
    [Display(Name = "Manga")]
    Manga = 1,
    
    /// <summary>Interactive environment layout focused on game mechanics and playability.</summary>
    [Display(Name = "Game")]
    Game = 2,
    
    /// <summary>Blended layouts for visual novels or interactive fiction.</summary>
    [Display(Name = "Hybrid")]
    Hybrid = 3
}

/// <summary>
/// Defines the overarching tone of a project, influencing pacing and narrative beat suggestions.
/// </summary>
public enum ToneEnum
{
    /// <summary>A neutral or baseline tone.</summary>
    [Display(Name = "Neutral")]
    Neutral = 0,
    
    /// <summary>Serious, grim, or dark narrative atmosphere.</summary>
    [Display(Name = "Dark")]
    Dark = 1,
    
    /// <summary>Positive, uplifting, or bright tone.</summary>
    [Display(Name = "Light")]
    Light = 2,
    
    /// <summary>Comedic or lighthearted atmosphere.</summary>
    [Display(Name = "Humorous")]
    Humorous = 3,
    
    /// <summary>Suspenseful, high-stakes, or anxiety-inducing tone.</summary>
    [Display(Name = "Tense")]
    Tense = 4,
    
    /// <summary>Emotional or romantic narrative atmosphere.</summary>
    [Display(Name = "Romantic")]
    Romantic = 5
}

/// <summary>
/// Defines the target demographic for a project, influencing content warnings and complexity levels.
/// </summary>
public enum AudienceEnum
{
    /// <summary>General audience, suitable for all ages.</summary>
    [Display(Name = "All Ages")]
    AllAges = 0,
    
    /// <summary>Content tailored for children (typically under 12).</summary>
    [Display(Name = "Children")]
    Children = 1,
    
    /// <summary>Content tailored for teenagers (12-17).</summary>
    [Display(Name = "Teen")]
    Teen = 2,
    
    /// <summary>Mature content intended for adults (18+).</summary>
    [Display(Name = "Adult")]
    Adult = 3
}

