using Anima.Api.Base.Interfaces;
using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.Base.Services;

/// <summary>
/// Provides a foundational base class for domain services that manage simple entities without complex metadata requirements.
/// It provides standard implementations for update and delete operations with built-in error handling and logging.
/// </summary>
/// <typeparam name="TEntity">The type of the managed entity.</typeparam>
public abstract class SimpleDomainService<TEntity> : DomainService, IDomainService<TEntity> 
    where TEntity : class
{
    /// <summary>
    /// Initializes a new instance of the <see cref="SimpleDomainService{TEntity}"/> class.
    /// </summary>
    /// <param name="coreServices">The provider for accessing core cross-cutting services.</param>
    /// <param name="logger">The logger for recording service-level events and errors.</param>
    protected SimpleDomainService(ICoreServicesProvider coreServices, ILogger logger) 
        : base(coreServices, logger) { }

    /// <summary>
    /// Gets the database context used by this service. Must be implemented by concrete services.
    /// </summary>
    /// <returns>The application database context.</returns>
    protected abstract DbContext GetDbContext();

    /// <summary>
    /// Creates a new entity. Must be implemented by concrete services.
    /// </summary>
    /// <param name="entity">The entity data to create.</param>
    /// <returns>An <see cref="ApiResponseDto{IdentityDto}"/> indicating success or failure.</returns>
    public abstract Task<ApiResponseDto<IdentityDto>> CreateAsync(TEntity entity);

    /// <summary>
    /// Updates an existing entity by executing the specialized update logic.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to update.</param>
    /// <param name="updateDto">The updated data for the entity.</param>
    /// <returns>An <see cref="ApiResponseDto{TEntity}"/> indicating success or failure.</returns>
    public async Task<ApiResponseDto<TEntity>> UpdateAsync(Guid id, TEntity updateDto)
    {
        try
        {
            var responseDto = await OnUpdateAsync(id, updateDto);
            if(!responseDto.Successful)
                _logger.LogError("Error during update of {EntityType} with id {entityId}", typeof(TEntity).Name, id);
            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during update of {EntityType}", typeof(TEntity).Name);
            return ApiResponseDto<TEntity>.ServerError("An error occurred during update.");
        }
    }

    /// <summary>
    /// Deletes an existing entity by executing the specialized deletion logic.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <returns>An <see cref="ApiResponseDto<IdentityDto}"/> indicating success or failure.</returns>
    public async Task<ApiResponseDto<IdentityDto>> DeleteAsync(Guid id)
    {
        try
        {
            var responseDto = await OnDeleteAsync(id);
             if(!responseDto.Successful)
                _logger.LogError("Error during delete of {EntityType} with id {entityId}", typeof(TEntity).Name, id);
            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during deletion of {EntityType}", typeof(TEntity).Name);
            return ApiResponseDto<IdentityDto>.ServerError("An error occurred during deletion.");
        }
    }

    /// <summary>
    /// Retrieves an entity by its unique identifier. Must be implemented by concrete services.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>An <see cref="ApiResponseDto<TEntity}"/> containing the found entity or an error response.</returns>
    public abstract Task<ApiResponseDto<TEntity>> GetAsync(Guid id);

    /// <summary>
    /// Performs the specialized update logic for the entity. Must be implemented by concrete services.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <param name="updateDto">The updated data for the entity.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task<ApiResponseDto<TEntity>> OnUpdateAsync(Guid id, TEntity updateDto);

    /// <summary>
    /// Performs the specialized deletion logic for the entity. Must be implemented by concrete services.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    protected abstract Task<ApiResponseDto<IdentityDto>> OnDeleteAsync(Guid id);
}