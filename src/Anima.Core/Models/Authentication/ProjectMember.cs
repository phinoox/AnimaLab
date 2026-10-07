namespace Anima.Core.Models.Authentication;

using System.ComponentModel.DataAnnotations;
using Anima.Core.Models.Projects;
using Anima.Core.Utils;

/// <summary>
/// Specifies the role of a member within a specific project, determining their access levels and permissions.
/// </summary>
public enum ProjectMemberRoleEnum
{
    /// <summary>The creator or primary owner of the project with full administrative control.</summary>
    Owner = 0,

    /// <summary>An administrator with high-level management privileges within the project.</summary>
    Admin = 1,

    /// <summary>A contributor capable of editing content and managing tasks.</summary>
    Editor = 2,

    /// <summary>A member whose primary role is to review progress and provide feedback.</summary>
    Reviewer = 3,

    /// <summary>A member with read-only access to the project's contents.</summary>
    Viewer = 4,

    /// <summary>The default role assigned if no specific role is specified.</summary>
    Default = 99
}

/// <summary>
/// Represents a user's membership and associated role within a specific project.
/// </summary>
[ModelDependency(typeof(Project), typeof(User))]
public class ProjectMember
{
    /// <summary>
    /// Gets or sets the unique identifier for this membership record.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated project.
    /// </summary>
    public Guid ProjectId { get; set; }

    /// <summary>
    /// Gets or sets the identifier of the associated user.
    /// </summary>
    public Guid UserId { get; set; }

    /// <summary>
    /// Gets or sets the role assigned to this member within the project.
    /// </summary>
    public ProjectMemberRoleEnum Role { get; set; }

    /// <summary>
    /// Gets or sets the timestamp when the user joined the project.
    /// </summary>
    public DateTime JoinedAt { get; set; }

    // Navigation
    /// <summary>
    /// Gets or sets the associated project entity.
    /// </summary>
    [Required]
    public virtual Project Project { get; set; } = null!;

    /// <summary>
    /// Gets or sets the associated user entity.
    /// </summary>
    [Required]
    public virtual User User { get; set; } = null!;
}
