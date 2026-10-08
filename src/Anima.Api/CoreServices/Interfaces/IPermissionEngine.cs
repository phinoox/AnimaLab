

using Anima.Core.Models.Authentication;

namespace Anima.Api.CoreServices.Interfaces;
public enum AccessResultStatus { Success,Allowed, Forbidden, NotFound, Unauthorized }

public record AccessResult(AccessResultStatus Status, string Message = "");

public interface IPermissionEngine
{
    // Purely logical: No knowledge of ApiResponseDto or Web/HTTP context
    Task<AccessResult> CheckAccessAsync(Guid userId, Guid projectId, ProjectMemberRoleEnum minRole);
    Task<ProjectMemberRoleEnum> GetUserRole(Guid userId, Guid projectId);
    Task<bool> IsTargetInProjectAsync(Guid projectId, Guid targetId);
}