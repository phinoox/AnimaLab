namespace Anima.Api.Infrastructure.Commands;

using Anima.Core.Base.Commands;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using Anima.Api.CoreServices.Interfaces;
using Anima.Data.Contexts;

/// <summary>
/// The foundation for all handlers. Provides access to core services, 
/// identity context, and authorization helpers without managing transactions.
/// </summary>
public abstract class BaseCommandHandler<TCommand, TResponse> : IApiCommandHandler<TCommand, TResponse>
    where TCommand : IApiCommand
    where TResponse : class
{
    protected readonly ILogger _logger;
    protected readonly AppDbContext _db;
    protected readonly ICoreServicesProvider _core;
    protected readonly Guid _userId;

    protected abstract PermissionsEnum RequiredPermission { get; }
    protected abstract ProjectMemberRoleEnum MinRole { get; }
    protected abstract IdentityRequirementEnum IdentityRequirement { get; }
    protected abstract ScopeRequirementEnum ScopeRequirement { get; }

    protected BaseCommandHandler(ILogger logger, AppDbContext db, ICoreServicesProvider core)
    {
        _logger = logger;
        _db = db;
        _core = core;
        _userId = _core.UserContext == null ? Guid.Empty : _core.UserContext.UserId ?? Guid.Empty;
    }

    public abstract Task<ApiResponseDto<TResponse>> HandleAsync(TCommand command, RequestContext context);

    // --- Authorization Helpers (Shared by Read and Write) ---

    protected async Task<ApiResponseDto<TResponse>?> ValidateIdentityAsync()
    {
        bool isLoggedIn = _userId != Guid.Empty;
        if (IdentityRequirement == IdentityRequirementEnum.MustBeUnauthenticated && isLoggedIn)
            return ApiResponseDto<TResponse>.BadRequest("Already logged in");
        
        if (IdentityRequirement == IdentityRequirementEnum.MustBeAuthenticated && !isLoggedIn)
            return ApiResponseDto<TResponse>.Unauthorized("You need to be logged in");

        return null;
    }

    protected async Task<ApiResponseDto<TResponse>?> ValidateScopeAsync(RequestContext context)
    {
        return ScopeRequirement switch
        {
            ScopeRequirementEnum.Project => await CheckProjectAccess(context.ProjectId),
            ScopeRequirementEnum.Entity => await CheckEntityAccess(context.ProjectId, context.EntityId),
            _ => null
        };
    }

    protected async Task<ApiResponseDto<TResponse>?> CheckProjectAccess(Guid? projectId)
    {
        if (projectId == null) return ApiResponseDto<TResponse>.BadRequest("Project ID missing in route");
        return await CheckAccessAsync<TResponse>(projectId.Value, RequiredPermission, MinRole);
    }

    protected async Task<ApiResponseDto<TResponse>?> CheckEntityAccess(Guid? projectId, Guid? entityId)
    {
        if (projectId == null || entityId == null) 
            return ApiResponseDto<TResponse>.BadRequest("Scope IDs missing in route");
        return await CheckAccessAsync<TResponse>(projectId.Value, RequiredPermission, MinRole);
    }

    protected async Task<ApiResponseDto<T>?> CheckAccessAsync<T>(Guid projectId, PermissionsEnum permissions, ProjectMemberRoleEnum minRole) where T : class
    {
        var targetRole = minRole == ProjectMemberRoleEnum.Default ? permissions switch
        {
            PermissionsEnum.CanDelete => ProjectMemberRoleEnum.Admin,
            PermissionsEnum.CanView => ProjectMemberRoleEnum.Viewer,
            PermissionsEnum.CanEdit => ProjectMemberRoleEnum.Editor,
            PermissionsEnum.CanCreate => ProjectMemberRoleEnum.Admin,
            _ => ProjectMemberRoleEnum.Owner
        } : minRole;

        var result = await _core.PermissionEngine.CheckAccessAsync(_userId, projectId, targetRole);

        return result.Status switch
        {
            AccessResultStatus.Allowed => null,
            AccessResultStatus.NotFound => ApiResponseDto<T>.NotFound(result.Message),
            AccessResultStatus.Forbidden => ApiResponseDto<T>.Forbidden(result.Message),
            AccessResultStatus.Unauthorized => ApiResponseDto<T>.Unauthorized(result.Message),
            _ => ApiResponseDto<T>.Unauthorized(result.Message)
        };
    }

    // --- Utility Helpers (Shared by Read and Write) ---

     protected async Task MapProperties<TSource>(TSource source, TSource target, Guid? projectId = null) where TSource : class
    {
        await MapProperties<TSource,TSource>(source,target,projectId);
    }


    protected async Task MapProperties<TSource, TTarget>(TSource source, TTarget target, Guid? projectId = null) where TTarget : class
    {
        if (_userId == Guid.Empty) return;
        var role = ProjectMemberRoleEnum.Default;
        if(projectId != null)
            role = await _core.PermissionEngine.GetUserRole(_userId, projectId.Value);
        _core.EntityMapper.Map(source, target, role);
    }

    public async Task<bool> CheckIsLoggedIn() => _userId != Guid.Empty;
}