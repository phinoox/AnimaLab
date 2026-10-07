// =============================================================================
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Anima.Core.Models.Projects;

/// <summary>
/// Defines the lifecycle stages of a project.
/// </summary>
public enum ProjectStatusEnum
{
    /// <summary>Initial stage; content is being conceptualized or drafted.</summary>
    Draft = 0,
    
    /// <summary>Active development phase; work is ongoing.</summary>
    InProgress = 1,
    
    /// <summary>The project is complete and available for public viewing/use.</summary>
    Published = 2,
    
    /// <summary>The project has been moved to long-term storage or is no longer active.</summary>
    Archived = 3,

    /// <summary>The project has been marked as deleted (soft delete).</summary>
    Deleted = 4
}