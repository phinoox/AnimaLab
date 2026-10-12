namespace Anima.Core.Models.Authentication;

using System.ComponentModel.DataAnnotations;
using Anima.Core.Models.Projects;
using Anima.Core.Utils;


/// <summary>
/// Represents a user's membership and associated role within a specific project.
/// </summary>
[ModelDependency(typeof(Project), typeof(User))]
public class ProjectMember : EntityBase
{
   
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
