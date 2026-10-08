using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Authentication;

namespace Anima.Api.Base.Services;

/// <summary>
/// Provides the foundational base class for all domain-level services in the application.
/// It provides access to core cross-cutting concerns and common authorization/logging utilities.
/// </summary>
public abstract class DomainService
{
    protected ICoreServicesProvider _core;
    protected readonly ILogger _logger;
    protected Guid _userId;

    /// <summary>
    /// Initializes a new instance of the <see cref="DomainService"/> class.
    /// </summary>
    /// <param name="coreServices">The provider for accessing core cross-cutting services.</param>
    /// <param name="logger">The logger for recording service-level events and errors.</param>
    public DomainService(ICoreServicesProvider coreServices, ILogger logger)
    {
        _core = coreServices;
        _userId = _core.UserContext == null ? Guid.Empty : _core.UserContext.UserId ?? Guid.Empty;
        _logger = logger;
    }

    /// <summary>
    /// Logs a database-level activity using the core audit service.
    /// </summary>
    /// <param name="projectId">The ID of the project associated with the action.</param>
    /// <param name="action">A description of the action performed.</param>
    /// <param name="relatedEntityType">The type of entity being acted upon.</param>
    /// <param name="relatedEntityId">The unique identifier of the related entity.</param>
    /// <param name="description">Additional context for the audit log.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected async Task LogDbAsync(Guid projectId, string action, string relatedEntityType, Guid relatedEntityId, string description)
    {
        await _core.AuditService.LogDbAsync(projectId, action, relatedEntityType, relatedEntityId, description);
    }

    /// <summary>
    /// Checks if the current user is authenticated and has a valid identity in the context.
    /// </summary>
    /// <returns>True if the user ID is not empty; otherwise, false.</returns>
    public async Task<bool> CheckIsLoggedIn()
    {
        return _userId != Guid.Empty;
    }

    /// <summary>
    /// Verifies if a specific target entity belongs to the specified project scope.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="targetId">The unique identifier of the target entity.</param>
    /// <returns>True if the target is within the project; otherwise, false.</returns>
    protected async Task<bool> IsTargetInProjectAsync(Guid projectId, Guid targetId)
    {
        return await _core.PermissionEngine.IsTargetInProjectAsync(projectId, targetId);
    }

    /// <summary>
    /// Checks if the current user has permission to perform a specific action on a resource.
    /// This is a convenience wrapper for the core PermissionEngine.
    /// </summary>
    /// <typeparam name="T">The type of the expected successful response DTO.</typeparam>
    /// <param name="projectId">The ID of the project being accessed.</param>
    /// <param name="PermissionsEnum">The permission level required for the action.</param>
    /// <param name="minRole">The minimum role required. If default, it is inferred from PermissionsEnum.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> indicating success or a specific authorization error.</returns>
    public async Task<ApiResponseDto<T>> CheckAccessAsync<T>(Guid? projectId, PermissionsEnum PermissionsEnum, ProjectMemberRoleEnum minRole = ProjectMemberRoleEnum.Default) where T : class
    {
        return await CheckAccessAsync<T>(projectId.Value, PermissionsEnum, minRole);
    }

    /// <summary>
    /// Evaluates authorization by mapping the required permission level to a user role and checking it against the PermissionEngine.
    /// </summary>
    /// <typeparam name="T">The type of the expected successful response DTO.</typeparam>
    /// <param name="projectId">The ID of the project being accessed.</param>
    /// <param name="PermissionsEnum">The permission level required for the action.</param>
    /// <param name="minRole">The minimum role required. If default, it is inferred from PermissionsEnum.</param>
    /// <returns>An <see cref="ApiResponseDto{T}"/> indicating success or a specific authorization error.</returns>
    public async Task<ApiResponseDto<T>> CheckAccessAsync<T>(Guid projectId, PermissionsEnum PermissionsEnum, ProjectMemberRoleEnum minRole = ProjectMemberRoleEnum.Default) where T : class
    {
        // Determine the required role if not explicitly provided
        if (minRole == ProjectMemberRoleEnum.Default)
        {
            minRole = PermissionsEnum switch
            {
                PermissionsEnum.CanDelete => ProjectMemberRoleEnum.Admin,
                PermissionsEnum.CanView => ProjectMemberRoleEnum.Viewer,
                PermissionsEnum.CanEdit => ProjectMemberRoleEnum.Editor,
                PermissionsEnum.CanCreate => ProjectMemberRoleEnum.Admin,
                _ => ProjectMemberRoleEnum.Owner
            };
        }

        var result = await _core.PermissionEngine.CheckAccessAsync(_userId, projectId, minRole);

        return result.Status switch
        {
            AccessResultStatus.Allowed => ApiResponseDto<T>.Success(default!),
            AccessResultStatus.NotFound => ApiResponseDto<T>.NotFound(result.Message),
            AccessResultStatus.Forbidden => ApiResponseDto<T>.Forbidden(result.Message),
            AccessResultStatus.Unauthorized => ApiResponseDto<T>.Unauthorized(result.Message),
            _ => ApiResponseDto<T>.Unauthorized(result.Message)
        };
    }

    /// <summary>
    /// Checks if the current user has Admin rights for the project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>True if access is allowed; otherwise, false.</returns>
    public async Task<bool> IsAdminAsync(Guid projectId)
    {
        if (_userId == Guid.Empty) return false;

        var result = await _core.PermissionEngine.CheckAccessAsync(_userId, projectId, ProjectMemberRoleEnum.Admin);
        return result?.Status == AccessResultStatus.Allowed;
    }

    /// <summary>
    /// Maps properties from a source DTO to a target entity using the core EntityMapper.
    /// Respects [ReadOnly], [Sanitize], and [Validate] attributes on the target.
    /// </summary>
    /// <typeparam name="TSource">The type of the source DTO.</typeparam>
    /// <typeparam name="TTarget">The type of the destination entity.</typeparam>
    /// <param name="source">The source object containing data.</param>
    /// <param name="target">The target object to be updated.</param>
    /// <param name="projectId">The ID of the project context for permission validation.</param>
    protected async Task MapProperties<TSource, TTarget>(TSource source, TTarget target, Guid projectId) where TTarget : class
    {
        if (_userId == Guid.Empty)
            return;

        var role = await _core.PermissionEngine.GetUserRole(_userId, projectId);
        _core.EntityMapper.Map(source, target);
    }
}