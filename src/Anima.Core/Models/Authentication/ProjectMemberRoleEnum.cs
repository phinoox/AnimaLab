namespace Anima.Core.Models.Authentication;

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
