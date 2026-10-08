using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Projects;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Anima.Api.Base;

public abstract class EntityDomainService<TMeta, TEntity> : DomainService , IEntityDomainService<TMeta,TEntity>
    where TEntity : MetaEntityBase<TMeta>
    where TMeta : BaseMetaInfo,new ()
{
    protected readonly AppDbContext _db;

    protected EntityDomainService(ICoreServicesProvider coreServices, ILogger logger,AppDbContext db) 
        : base(coreServices, logger) {_db = db; }

     // NEW: The concrete service must provide its context so the base can manage transactions
    

    public async Task<ApiResponseDto<TEntity>> GetAsync(string projectIdentifier,string entityIdentifier)
    {
         var projectId = await _core.SlugResolverService.ResolveSlugAsync<ProjectMetaInfo>(projectIdentifier);
        if(projectId == null || projectId == Guid.Empty)
            return ApiResponseDto<TEntity>.NotFound($"could not find project with identifier {projectIdentifier}");

        var entityId = await _core.SlugResolverService.ResolveSlugAsync<TMeta>(entityIdentifier);
        if(entityId == null || entityId == Guid.Empty)
            return ApiResponseDto<TEntity>.NotFound($"could not find project with identifier {entityIdentifier}");

        return await GetAsync(projectId.Value,entityId.Value);
    }

    public async Task<ApiResponseDto<CreateResponseDto>> CreateAsync(string projectIdentifier, TMeta metaInfoDto)
    {
        var projectId = await _core.SlugResolverService.ResolveSlugAsync<ProjectMetaInfo>(projectIdentifier);
        if(projectId == null || projectId == Guid.Empty)
            return ApiResponseDto<CreateResponseDto>.NotFound($"could not find project with identifier {projectIdentifier}");
        // 1. PermissionsEnum Check (using the default CanCreate PermissionsEnum for this entity type)
        var authorizationError = await CheckAccessAsync<CreateResponseDto>(projectId, PermissionsEnum.CanCreate);
        if (authorizationError != null) 
            return authorizationError;

        IDbContextTransaction? transaction = null;

        // 2. Execute the specialized creation logic
        try
        {
            
            transaction = await _db.Database.BeginTransactionAsync();

            var metaInfo = await CreateMetaInfoAsync(projectId.Value,metaInfoDto);

            await MapProperties(metaInfo,metaInfoDto,projectId.Value);
            
            var responseDto = await OnCreateAsync(projectId.Value, metaInfo);
            if(responseDto.Successful)
             await transaction.CommitAsync();
            else
             await transaction.RollbackAsync();
             
            return responseDto;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error during creation of {EntityType}", typeof(TEntity).Name);
            return ApiResponseDto<CreateResponseDto>.ServerError("An error occurred during creation.");
        }
    }

    public async Task<ApiResponseDto<TEntity>> UpdateAsync(string projectIdentifier,string entityIdentifier, TEntity updateDto)
    {
        var projectId = await _core.SlugResolverService.ResolveSlugAsync<ProjectMetaInfo>(projectIdentifier);
        if(projectId == null || projectId == Guid.Empty)
            return ApiResponseDto<TEntity>.NotFound($"could not find project with identifier {projectIdentifier}");

        var entityId = await _core.SlugResolverService.ResolveSlugAsync<TMeta>(entityIdentifier);
        if(entityId == null || entityId == Guid.Empty)
            return ApiResponseDto<TEntity>.NotFound($"could not find project with identifier {entityIdentifier}");


        // 1. PermissionsEnum Check
        var authorizationError = await CheckAccessAsync<TEntity>(projectId, PermissionsEnum.CanEdit);
        if (authorizationError != null)
            return authorizationError;


        // 2. Execute the specialized update logic
        try
        {
            var responseDto = await OnUpdateAsync(projectId.Value, entityId.Value, updateDto);
            if (!responseDto.Successful)
                _logger.LogError("Error during update of {EntityType} with id {entityId}", typeof(TEntity).Name);
            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during update of {EntityType}", typeof(TEntity).Name);
            return ApiResponseDto<TEntity>.ServerError("An error occurred during update.");
        }
    }

    public async Task<ApiResponseDto<TMeta>> UpdateMetaInfoAsync(string projectIdentifier,string entityIdentifier, TMeta metaDto)
    {
         var projectId = await _core.SlugResolverService.ResolveSlugAsync<ProjectMetaInfo>(projectIdentifier);
        if(projectId == null || projectId == Guid.Empty)
            return ApiResponseDto<TMeta>.NotFound($"could not find project with identifier {projectIdentifier}");

        var entityId = await _core.SlugResolverService.ResolveSlugAsync<TMeta>(entityIdentifier);
        if(entityId == null || entityId == Guid.Empty)
            return ApiResponseDto<TMeta>.NotFound($"could not find project with identifier {entityIdentifier}");


         var authorizationError = await CheckAccessAsync<TMeta>(projectId, PermissionsEnum.CanEdit);
        if (authorizationError != null)
            return authorizationError;

        try
        {
            // 2. Execute the specialized metadata-only update
            var result = await OnUpdateMetaAsync(projectId.Value, entityId.Value, metaDto);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating MetaInfo for {EntityType} with id {entityId}", typeof(TEntity).Name, entityId);
            return ApiResponseDto<TMeta>.ServerError("An error occurred during metadata update.");
        }
    }


    public async Task<ApiResponseDto<IdentityDto>> DeleteAsync(string projectIdentifier,string entityIdentifier)
    {
         var projectId = await _core.SlugResolverService.ResolveSlugAsync<ProjectMetaInfo>(projectIdentifier);
        if(projectId == null || projectId == Guid.Empty)
            return ApiResponseDto<IdentityDto>.NotFound($"could not find project with identifier {projectIdentifier}");

        var entityId = await _core.SlugResolverService.ResolveSlugAsync<TMeta>(entityIdentifier);
        if(entityId == null || entityId == Guid.Empty)
            return ApiResponseDto<IdentityDto>.NotFound($"could not find project with identifier {entityIdentifier}");


        // 1. PermissionsEnum Check
        var authorizationError = await CheckAccessAsync<IdentityDto>(projectId, PermissionsEnum.CanDelete);
        if (authorizationError != null)
            return authorizationError;

        // 2. Execute the specialized deletion logic
        try
        {
            var responseDto = await OnDeleteAsync(projectId.Value, entityId.Value);
            if (!responseDto.Successful)
                _logger.LogError("Error during delete of {EntityType} with id {entityId}", typeof(TEntity).Name);
            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during deletion of {EntityType}", typeof(TEntity).Name);
            return ApiResponseDto<IdentityDto>.ServerError("An error occurred during deletion.");
        }
    }

    // --- Abstract Hooks (The Implementation Layer) ---
    // Concrete services like StoryService will override these.

    protected abstract Task<TMeta> CreateMetaInfoAsync(Guid projectId, TMeta metaDto);

    protected abstract Task<ApiResponseDto<TMeta>> OnUpdateMetaAsync(Guid projectId,Guid entityId, TMeta metaDto);

    protected abstract  Task<ApiResponseDto<CreateResponseDto>> OnCreateAsync(Guid projectId, in TMeta metaInfo);
    protected abstract  Task<ApiResponseDto<TEntity>> OnUpdateAsync(Guid projectId,Guid entityId, TEntity updateDto);
    protected abstract  Task<ApiResponseDto<IdentityDto>> OnDeleteAsync(Guid projectId, Guid entityId);

    // Get and Resolve are typically implemented directly in the concrete service 
    // because they don't always follow a standard "PermissionsEnum -> Action" flow.
    protected abstract Task<ApiResponseDto<TEntity>> GetAsync(Guid projectId, Guid entityId);
}