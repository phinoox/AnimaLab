using Anima.Core.Dtos.Shared;

namespace Anima.Api.Base;

/// <summary>
/// Defines a contract for domain services that manage entities with associated metadata.
/// These services handle operations using both project and entity identifiers (slugs).
/// </summary>
/// <typeparam name="TMeta">The type of the meta information associated with the entity.</typeparam>
/// <typeparam name="TEntity">The type of the managed entity.</typeparam>
public interface IEntityDomainService<TMeta, TEntity>
    where TMeta : BaseMetaInfo
    where TEntity : MetaEntityBase<TMeta>
{
    /// <summary>
    /// Creates a new entity within the specified project.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="metaInfo">The metadata used to create the entity.</param>
    /// <returns>An <see cref="ApiResponseDto{CreateResponseDto}"/> indicating success or failure.</returns>
    Task<ApiResponseDto<CreateResponseDto>> CreateAsync(string projectIdentifier, TMeta metaInfo);

    /// <summary>
    /// Retrieves an entity by resolving its slug within the specified project.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="entityIdentifier">The slug of the target entity.</param>
    /// <returns>An <see cref="ApiResponseDto{TEntity}"/> containing the found entity or an error response.</returns>
    Task<ApiResponseDto<TEntity>> GetAsync(string projectIdentifier, string entityIdentifier);

    /// <summary>
    /// Updates an existing entity by resolving its slug within the specified project.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="entityIdentifier">The slug of the target entity.</param>
    /// <param name="updateDto">The updated data for the entity.</param>
    /// <returns>An <see cref="ApiResponseDto{TEntity}"/> indicating success or failure.</returns>
    Task<ApiResponseDto<TEntity>> UpdateAsync(string projectIdentifier, string entityIdentifier, TEntity updateDto);

    /// <summary>
    /// Updates only the metadata (the "Soul") of the entity.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="entityIdentifier">The slug of the target entity.</param>
    /// <param name="metaDto">The new metadata to apply.</param>
    /// <returns>An <see cref="ApiResponseDto{TMeta}"/> indicating success or failure.</returns>
    Task<ApiResponseDto<TMeta>> UpdateMetaInfoAsync(string projectIdentifier, string entityIdentifier, TMeta metaDto);

    /// <summary>
    /// Deletes an existing entity by resolving its slug within the specified project.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="entityIdentifier">The slug of the target entity.</param>
    /// <returns>An <see cref="ApiResponseDto<IdentityDto}"/> indicating success or failure.</returns>
    Task<ApiResponseDto<IdentityDto>> DeleteAsync(string projectIdentifier, string entityIdentifier);
}