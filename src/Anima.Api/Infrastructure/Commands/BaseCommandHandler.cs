namespace Anima.Api.Infrastructure.Commands;

using Anima.Core.Base.Commands;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore;
using Anima.Api.CoreServices.Interfaces;
using Anima.Data.Contexts;

/// <summary>
/// Provides the foundational base class for all command handlers, managing the transaction lifecycle, 
/// error handling, and providing direct access to core domain tools (Permissions, Mapping, Logging).
/// </summary>
public abstract class BaseCommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand
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

    /// <summary>
    /// The template method that manages the transaction lifecycle and error handling.
    /// It automatically performs permission checks based on the command's requirements.
    /// </summary>
    public async Task<ApiResponseDto<TResponse>> HandleAsync(TCommand command,RequestContext context) 
    {
        // 1. Check Identity Requirement first
        var identityError = await ValidateIdentityAsync();
        if (identityError != null) return identityError;

        // 2. Check Scope/Permission requirement second
        var scopeError = await ValidateScopeAsync(context);
        if (scopeError != null) return scopeError;

        IDbContextTransaction? transaction = null;

        try
        {
            transaction = await _db.Database.BeginTransactionAsync();

            // 2. Execute the actual business logic provided by the concrete handler
            var response = await ExecuteAsync(command,context);

            if (!response.Successful)
            {
                await transaction.RollbackAsync();
                _logger.LogWarning("Command {CommandName} failed execution logic. Transaction rolled back.", typeof(TCommand).Name);
            }
            else
            {
                await transaction.CommitAsync();
            }

            return response;
        }
        catch (Exception ex)
        {
            if (transaction != null) await transaction.RollbackAsync();
            _logger.LogError(ex, "Error executing command {CommandName}. Transaction rolled back.", typeof(TCommand).Name);
            throw; 
        }
        finally
        {
            if (transaction != null) await transaction.DisposeAsync();
        }
    }
   
   private async Task<ApiResponseDto<TResponse>?> ValidateIdentityAsync()
    {
        bool isLoggedIn = _userId != Guid.Empty;
        if (IdentityRequirement == IdentityRequirementEnum.MustBeUnauthenticated && isLoggedIn)
            return ApiResponseDto<TResponse>.BadRequest("Already logged in");
        
        if (IdentityRequirement == IdentityRequirementEnum.MustBeAuthenticated && !isLoggedIn)
            return ApiResponseDto<TResponse>.Unauthorized("You need to be logged in");

        return null;
    }

     private async Task<ApiResponseDto<TResponse>?> ValidateScopeAsync(RequestContext context)
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
        
        // Perform the deep check: Does this entity belong to this project?
        return await CheckAccessAsync<TResponse>(projectId.Value, RequiredPermission, MinRole);
    }

    /// <summary>
    /// usually this means we just need to see if the user is not logged in
    /// </summary>
    /// <param name="command"></param>
    /// <returns></returns>
    private async Task<ApiResponseDto<TResponse>?> CheckPermissionAsync(ICommand command)
    {
         if(_userId != Guid.Empty)
            return ApiResponseDto<TResponse>.BadRequest("Already logged in");
        return null;
    }

    

    /// <summary>
    /// Evaluates authorization by mapping the required permission level to a user role and checking it against the PermissionEngine.
    /// </summary>
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

    protected abstract Task<ApiResponseDto<TResponse>> ExecuteAsync(TCommand command,RequestContext context);

    // --- Core Domain Tools (Extracted from DomainService) ---

    protected async Task LogDbAsync(Guid projectId, string action, string relatedEntityType, Guid relatedEntityId, string description)
    {
        await _core.AuditService.LogDbAsync(projectId, action, relatedEntityType, relatedEntityId, description);
    }

    public async Task<bool> CheckIsLoggedIn() => _userId != Guid.Empty;

    protected async Task<bool> IsTargetInProjectAsync(Guid projectId, Guid targetId)
    {
        return await _core.PermissionEngine.IsTargetInProjectAsync(projectId, targetId);
    }

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
}

public interface ISuccessfulCommandResponse
{
    bool IsSuccessful { get; }
}

