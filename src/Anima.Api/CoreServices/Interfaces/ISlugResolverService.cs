using Anima.Core.Models.Shared;

namespace Anima.Api.CoreServices.Interfaces;

/// <summary>
/// Defines a contract for a service that resolves unique, URL-friendly identifiers (slugs) to their corresponding entity IDs.
/// </summary>
public interface ISlugResolverService
{
    /// <summary>
    /// Resolves a slug to an entity ID within a global scope.
    /// </summary>
    /// <typeparam name="TMeta">The type of the meta information associated with the target entity.</typeparam>
    /// <param name="identifier">The string identifier (slug) to resolve.</param>
    /// <returns>A task representing the asynchronous operation, returning the unique ID of the resolved entity if found; otherwise, null.</returns>
    Task<Guid?> ResolveSlugAsync<TMeta>(string identifier) where TMeta : BaseMetaInfo;

    /// <summary>
    /// Resolves a slug to an entity ID within a specific project scope.
    /// </summary>
    /// <typeparam name="TMeta">The type of the meta information associated with the target entity.</typeparam>
    /// <param name="projectId">The ID of the project providing the scoping context.</param>
    /// <param name="identifier">The string identifier (slug) to resolve.</param>
    /// <returns>A task representing the asynchronous operation, returning the unique ID of the resolved entity if found within the project; otherwise, null.</returns>
    Task<Guid?> ResolveSlugAsync<TMeta>(Guid projectId, string identifier) where TMeta : ProjectScopedMetaInfo;
}