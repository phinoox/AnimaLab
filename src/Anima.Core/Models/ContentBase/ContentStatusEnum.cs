// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Anima.Core.Models.ContentBase;

/// <summary>
/// Represents the current lifecycle status of a content item.
/// </summary>
public enum ContentStatusEnum
{
    /// <summary>The content is in an initial draft state.</summary>
    Draft = 0,
    
    /// <summary>The content is actively being developed or edited.</summary>
    InProgress = 1,
    
    /// <summary>The content has been officially published.</summary>
    Published = 2,
    
    /// <summary>The content has been moved to an archive state.</summary>
    Archived = 3,
    
    /// <summary>The content is currently undergoing a review process.</summary>
    UnderReview = 4,
    
    /// <summary>The content has reached its final, completed state.</summary>
    Completed = 5
}