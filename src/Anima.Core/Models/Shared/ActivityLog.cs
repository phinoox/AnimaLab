using Anima.Core.Models.Authentication;
using Anima.Core.Models.Projects;

namespace Anima.Core.Models.Shared;

/// <summary>
/// Represents a business-level audit event within a project, used for accountability and history tracking.
/// </summary>
[ModelDependency(typeof(Project), typeof(User))]
public class ActivityLog
{
    /// <summary>
    /// Gets or sets the unique identifier for this activity log entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the identifier of the project associated with this activity.
    /// </summary>
   [Required] 
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the associated project entity.
    /// </summary>
    // Necessary for EF Core to perform Cascade Delete
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;

    /// <summary>
    /// Gets or sets the identifier of the user who performed this action.
    /// </summary>
    // Who did it
    [Required]
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the action performed (e.g., "Created", "Updated", "Deleted", "Revoked").
    /// </summary>
    // What happened (e.g., "Created", "Updated", "Deleted", "Revoked")
    [Required, MaxLength(64)]
    public string Action { get; set; } = "";

    /// <summary>
    /// Gets or sets the type of entity involved in this activity (e.g., "Character", "DialogueBranch", "Token").
    /// </summary>
    // The type of entity involved (e.g., "Character", "DialogueBranch", "Token")
    [Required, MaxLength(64)]
    public string RelatedEntityType { get; set; } = "";

    /// <summary>
    /// Gets or sets the identifier of the specific object being acted upon.
    /// </summary>
    // The ID of the specific object being acted upon
    public Guid? RelatedEntityId { get; set; }

    /// <summary>
    /// Gets or sets a detailed description of the the activity.
    /// </summary>
    [MaxLength(1024)]
    public string Description { get; set; } = "";

    /// <summary>
    /// Gets or sets the timestamp when this activity occurred.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}