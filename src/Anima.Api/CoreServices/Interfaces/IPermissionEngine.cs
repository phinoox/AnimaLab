using Anima.Core.Models.Authentication;

namespace Anima.Api.CoreServices.Interfaces;

/// <summary>
/// Defines the possible outcomes of a permission check.
/// </summary>
public enum AccessResultStatus { Success, Allowed, Forbidden, NotFound, Unauthorized }

/// <summary>
/// Represents the result of an access evaluation.
/// </summary>
/// <param name="Status">The status of the access request.</param>
/// <param name="Message">A descriptive message explaining the outcome (e.g., why access was forbidden).</param>
public record AccessResult(AccessResultStatus Status, string Message = "");

/// <summary>
/// Defines a contract for an engine that evaluates user permissions and access rights within project scopes.
/// This service is purely logical and has no knowledge of the transport layer (HTTP/Web).
/// </summary>
public interface IPermissionEngine
{
    /// <summary>
    /// Evaluates if a user has sufficient permission to perform an action within a specific project.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="projectId">The ID of the project being accessed.</param>
    /// <param name="minRole">The minimum required role to perform the action.</param>
    /// <returns>An <see cref="AccessResult"/> indicating whether access is granted or denied, and why.</returns>
    Task<AccessResult> CheckAccessAsync(Guid userId, Guid projectId, ProjectMemberRoleEnum minRole);

    /// <summary>
    /// Retrieves the specific role of a user within a given project.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>The <see cref="ProjectMemberRoleEnum"/> assigned to the user in that project.</returns>
    Task<ProjectMemberRoleEnum> GetUserRole(Guid userId, Guid projectId);

    /// <summary>
    /// Determines if a specific target entity belongs to the specified project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="targetId">The unique identifier of the entity being checked.</param>
    /// <returns>True if the target is within the project scope; otherwise, false.</returns>
    Task<bool> IsTargetInProjectAsync(Guid projectId, Guid targetId);
}