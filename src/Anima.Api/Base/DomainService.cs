using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Authentication;

namespace Anima.Api.Base;

public abstract class DomainService
{
    protected ICoreServicesProvider _core;
    protected readonly ILogger _logger;
    protected Guid _userId;

    public DomainService(ICoreServicesProvider coreServices,ILogger logger)
    {
        _core = coreServices;
        _userId = _core.UserContext == null ? Guid.Empty : _core.UserContext.UserId ?? Guid.Empty;
        _logger = logger;
    }

    protected async Task LogDbAsync(Guid projectId, string action, string relatedEntityType, Guid relatedEntityId, string description)
    {
        await _core.AuditService.LogDbAsync(projectId,action,relatedEntityType,relatedEntityId,description);
    }


    /// <summary>
    /// checks if the user is logged in. later could also check if the user is deleted or banned etc
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CheckIsLoggedIn()
    {
        return _userId != Guid.Empty;
    }

    protected async Task<bool> IsTargetInProjectAsync(Guid projectId, Guid targetId)
    {
        return await _core.PermissionEngine.IsTargetInProjectAsync(projectId,targetId);
       
    }


    public async Task<ApiResponseDto<T>> CheckAccessAsync<T>(Guid? projectId,PermissionsEnum PermissionsEnum, ProjectMemberRoleEnum minRole = ProjectMemberRoleEnum.Default) where T : class
    {
        return await CheckAccessAsync<T>(projectId.Value,PermissionsEnum,minRole);
    }



    /// <summary>
    /// Checks if the current user has PermissionsEnum to perform a specific action on a resource.
    /// This is the preferred method for all authorization checks.
    /// wrapper for permissionengine.checkpermissionasync for convinience
    /// </summary>
    public async Task<ApiResponseDto<T>> CheckAccessAsync<T>(Guid projectId,PermissionsEnum PermissionsEnum, ProjectMemberRoleEnum minRole = ProjectMemberRoleEnum.Default) where T : class
    {
        //ToDo: refine this once roles are more defined
        if(minRole == ProjectMemberRoleEnum.Default)
        {
            minRole = PermissionsEnum switch
            {
                PermissionsEnum.CanDelete =>  ProjectMemberRoleEnum.Admin,
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
    /// Useful for administrative overrides or permanent deletions.
    /// </summary>
    public async Task<bool> IsAdminAsync(Guid projectId)
    {
        if (_userId == Guid.Empty) return false;

        // We use the PermissionEngine directly via the gateway to check specifically for Admin role
        var result = await _core.PermissionEngine.CheckAccessAsync(_userId, projectId, ProjectMemberRoleEnum.Admin);
        return result?.Status == AccessResultStatus.Allowed;
    }
   
    /// <summary>
    /// Maps properties from a source DTO to a target entity using reflection-based mapping.
    /// Respects [ReadOnly], [Sanitize], and [Validate] attributes on the target.
    /// </summary>
    protected async Task MapProperties<TSource, TTarget>(TSource source, TTarget target,Guid projectId) where TTarget : class
    {
        if(_userId == Guid.Empty)
            return ;
        var role = await _core.PermissionEngine.GetUserRole(_userId,projectId);
        _core.EntityMapper.Map(source, target);
    }

}