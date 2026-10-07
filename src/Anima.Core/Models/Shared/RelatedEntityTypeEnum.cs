namespace Anima.Core.Models.Shared;

/// <summary>
/// Specifies the type of entity related to a system activity or event for cross-entity tracking.
/// </summary>
public enum RelatedEntityTypeEnum
{
    /// <summary>Represents content-related meta information (e.g., characters, worlds, mechanics).</summary>
    [Display(Name = "Content Item")]
    ContentMetaInfo = 0,

    /// <summary>Represents a project-related task or workflow item.</summary>
    [Display(Name = "Task")]
    ProjectTask = 1,

    /// <summary>Represents the overall project entity.</summary>
    [Display(Name = "Project")]
    Project = 2,

    /// <summary>Represents a narrative story sequence.</summary>
    [Display(Name = "Story Sequence")]
    StorySequence = 3,

    /// <summary>Represents an ability set or combat-related system component.</summary>
    [Display(Name = "Ability Set")]
    AbilitySet = 4,

    /// <summary>Represents a specific character entity.</summary>
    [Display(Name = "Character")]
    Character = 5,

    /// <summary>Represents an item within a game inventory system.</summary>
    [Display(Name = "Inventory Item")]
    InventoryItem = 6,

    /// <summary>Represents a project template entity.</summary>
    [Display(Name = "Template")]
    Template = 7,
}
