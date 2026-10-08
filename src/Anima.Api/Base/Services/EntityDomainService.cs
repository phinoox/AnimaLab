using Anima.Api.Base.Interfaces;
using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Projects;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore.Storage;

namespace Anima.Api.Base.Services;

/// <summary>
/// Provides a specialized base class for domain services that manage entities with associated metadata.
/// It handles slug-based resolution, permission checks, and transaction orchestration for CRUD operations.
/// </summary>
/// <typeparam name="TMeta">The type of the meta information associated with the entity.</typeparam>
/// <typeparam name="TEntity">The type of the managed entity.</typeparam>
public abstract class EntityDomainService<TMeta, TEntity> : DomainService, IEntityDomainService<TMeta, TEntity>
    where TEntity : MetaEntityBase<TMeta>
    where TMeta : BaseMetaInfo, new()
{
    protected readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityDomainService{TMeta, TEntity}"/> class.
    /// </summary>
    /// <param name="coreServices">The provider for accessing core cross-cutting services.</param>
    /// <param name="logger">The logger for recording service-level events and errors.</param>
    /// <param name="db">The application database context.</param>
    protected EntityDomainService(ICoreServicesProvider coreServices, ILogger logger, AppDbContext db) 
        : base(coreServices, logger) 
    { 
        _db = db; 
    }

    /// <summary>
    /// Retrieves an entity by resolving both the project and the entity via their slugs.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="entityIdentifier">The slug of the target entity.</param>
    /// <returns>An <see cref="ApiResponseDto{TEntity}"/> containing the found entity or an error response.</returns>
    public async Task<ApiResponseDto<TEntity>> GetAsync(string projectIdentifier, string entityIdentifier)
    {
        var projectId = await _core.SlugResolverService.ResolveSlugAsync<ProjectMetaInfo>(projectIdentifier);
        if (projectId == null || projectId == Guid.Empty)
            return ApiResponseDto<TEntity>.NotFound($"could not find project with identifier {projectIdentifier}");

        var entityId = await _core.SlugResolverService.ResolveSlugAsync<TMeta>(entityIdentifier);
        if (entityId == null || entityId == Guid.Empty)
            return ApiResponseDto<TEntity>.NotFound($"could not find project with identifier {entityIdentifier}");

        return await GetAsync(projectId.Value, entityId.Value);
    }

    /// <summary>
    /// Orchestrates the creation of a new entity, including slug resolution, permission checks, and transaction management.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="metaInfoDto">The metadata used to create the entity.</param>
    /// <returns>An <see cref="ApiResponseDto<CreateResponseDto}"/> indicating success or failure.</returns>
    public async Task<ApiResponseDto<CreateResponseDto>> CreateAsync(string projectIdentifier, TMeta metaInfoDto)
    {
        var projectId = await _core.SlugResolverService.ResolveSlugAsync<ProjectMetaInfo>(projectIdentifier);
        if (projectId == null || projectId == Guid.Empty)
            return ApiResponseDto<CreateResponseDto>.NotFound($"could not find project with identifier {projectIdentifier}");

        // 1. Permission Check (using the default CanCreate PermissionsEnum for this entity type)
        var authorizationError = await CheckAccessAsync<CreateResponseDto>(projectId, PermissionsEnum.CanCreate);
        if (authorizationError != null) 
            return authorizationError;

        IDbContextTransaction? transaction = null;

        // 2. Execute the specialized creation logic within a transaction
        try
        {
            transaction = await _db.Database.BeginTransactionAsync();

            var metaInfo = await CreateMetaInfoAsync(projectId.Value, metaInfoDto);

            await MapProperties(metaInfo, metaInfoDto, projectId.Value);
            
            var responseDto = await OnCreateAsync(projectId.Value, metaInfo);
            if (responseDto.Successful)
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

    /// <summary>
    /// Updates an existing entity by resolving identifiers via slugs and checking permissions.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="entityIdentifier">The slug of the target entity.</param>
    /// <param name="updateDto">The updated data for the entity.</param>
    /// <returns>An <see cref="ApiResponseDto{TEntity}"/> indicating success or failure.</returns>
    public async Task<ApiResponseDto<TEntity>> UpdateAsync(string projectIdentifier, string entityIdentifier, TEntity updateDto)
    {
        var projectId = await _core.SlugResolverService.ResolveSlugAsync<ProjectMetaInfo>(projectIdentifier);
        if (projectId == null || projectId == Guid.Empty)
            return ApiResponseDto<TEntity>.NotFound($"could not find project with identifier {projectIdentifier}");

        var entityId = await _core.SlugResolverService.ResolveSlugAsync<TMeta>(entityIdentifier);
        if (entityId == null || entityId == Guid.Empty)
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
                _logger.LogError("Error during update of {EntityType} with id {entityId}", typeof(TEntity).Name, entityId);
            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during update of {EntityType}", typeof(TEntity).Name);
            return ApiResponseDto<TEntity>.ServerError("An error occurred during update.");
        }
    }

    /// <summary>
    /// Updates only the metadata associated with an entity.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="entityIdentifier">The slug of the target entity.</param>
    /// <param name="metaDto">The new metadata to apply.</param>
    /// <returns>An <see cref="ApiResponseDto{TMeta}"/> indicating success or failure.</returns>
    public async Task<ApiResponseDto<TMeta>> UpdateMetaInfoAsync(string projectIdentifier, string entityIdentifier, TMeta metaDto)
    {
        var projectId = await _core.SlugResolverService.ResolveSlugAsync<ProjectMetaInfo>(projectIdentifier);
        if (projectId == null || projectId == Guid.Empty)
            return ApiResponseDto<TMeta>.NotFound($"could not find project with identifier {projectIdentifier}");

        var entityId = await _core.SlugResolverService.ResolveSlugAsync<TMeta>(entityIdentifier);
        if (entityId == null || entityId == Guid.Empty)
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

    /// <summary>
    /// Deletes an entity by resolving identifiers via slugs and checking permissions.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="entityIdentifier">The slug of the target entity.</param>
    /// <returns>An <see cref="ApiResponseDto<IdentityDto}"/> indicating success or failure.</returns>
    public async Task<ApiResponseDto<IdentityDto>> DeleteAsync(string projectIdentifier, string entityIdentifier)
    {
        var projectId = await _core.SlugResolverService.ResolveSlugAsync<ProjectMetaInfo>(projectIdentifier);
        if (projectId == null || projectId == Guid.Empty)
            return ApiResponseDto<IdentityDto>.NotFound($"could not find project with identifier {projectIdentifier}");

        var entityId = await _core.SlugResolverService.ResolveSlugAsync<TMeta>(entityIdentifier);
        if (entityId == null || entityId == Guid.Empty)
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
                _logger.LogError("Error during delete of {EntityType} with id {entityId}", typeof(TEntity).Name, entityId);
            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during deletion of {EntityType}", typeof(TEntity).Name);
            return ApiResponseDto<IdentityDto>.ServerError("An error occurred during deletion.");
        }
    }

    // --- Abstract Hooks (The Implementation Layer) ---

    /// <summary>
    /// Creates the metadata instance for the entity. Must be implemented by concrete services.
    /// </summary>
    protected abstract Task<TMeta> CreateMetaInfoAsync(Guid projectId, TMeta metaDto);

    /// <summary>
    /// Performs a metadata-only update. Must be implemented by concrete services.
    /// </summary>
    protected abstract Task<ApiResponseDto<TMeta>> OnUpdateMetaAsync(Guid projectId, Guid entityId, TMeta metaDto);

    /// <summary>
    /// Executes the specialized creation logic after metadata has been mapped.
    /// </summary>
    protected abstract Task<ApiResponseDto<CreateResponseDto>> OnCreateAsync(Guid projectId, in TMeta metaInfo);

    /// <summary>
    /// Executes the specialized update logic for the entity.
    /// </summary>
    protected abstract Task<ApiResponseDto<TEntity>> OnUpdateAsync(Guid projectId, Guid entityId, TEntity updateDto);

    /// <summary>
    /// Executes the specialized deletion logic for the entity.
    /// </summary>
    protected abstract Task<ApiResponseDto<IdentityDto>> OnDeleteAsync(Guid projectId, Guid entityId);

    /// <summary>
    /// Retrieves an entity by its numeric ID. Must be implemented by concrete services.
    /// </summary>
    protected abstract Task<ApiResponseDto<TEntity>> GetAsync(Guid projectId, Guid entityId);
}