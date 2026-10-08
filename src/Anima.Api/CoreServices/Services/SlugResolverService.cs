using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Models.Shared;
using Anima.Data.Contexts;

namespace Anima.Api.CoreServices.Services;

/// <summary>
/// Provides an implementation of <see cref="ISlugResolverService"/> that resolves slugs to entity IDs 
/// using the application's database context.
/// </summary>
public class SlugResolverService : ISlugResolverService
{
    private AppDbContext _db;

    public SlugResolverService(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Resolves a slug to an entity ID within a global scope.
    /// </summary>
    /// <typeparam name="TMeta">The type of the meta information associated with the target entity.</typeparam>
    /// <param name="identifier">The string identifier (slug) to resolve.</param>
    /// <returns>A task representing the asynchronous operation, returning the unique ID of the resolved entity if found; otherwise, null.</returns>
    public async Task<Guid?> ResolveSlugAsync<TMeta>(string identifier) where TMeta : BaseMetaInfo
    {
        if (Guid.TryParse(identifier, out var guid))
        {
            return guid;
        }

        var miSet = _db.Set<TMeta>();
        if (miSet == null)
            return null;
        var projectMetaInfo = miSet.FirstOrDefault(mi => mi.Slug == identifier);
        if (projectMetaInfo != null)
        {
            return (Guid?)projectMetaInfo.Id;
        }
        return null;
    }

    /// <summary>
    /// Resolves a slug to an entity ID within a specific project scope.
    /// </summary>
    /// <typeparam name="TMeta">The type of the meta information associated with the target entity.</typeparam>
    /// <param name="projectId">The ID of the project providing the scoping context.</param>
    /// <param name="identifier">The string identifier (slug) to resolve.</param>
    /// <returns>A task representing the asynchronous operation, returning the unique ID of the resolved entity if found within the project; otherwise, null.</returns>
    public async Task<Guid?> ResolveSlugAsync<TMeta>(Guid projectId, string identifier) where TMeta : ProjectScopedMetaInfo
    {
        if (Guid.TryParse(identifier, out var guid))
        {
            return guid;
        }
        
        var miSet = _db.Set<TMeta>();
        if (miSet == null)
            return null;
        var projectMetaInfo = miSet.FirstOrDefault(mi => mi.Slug == identifier && mi.ProjectId == projectId);
        if (projectMetaInfo != null)
        {
            return (Guid?)projectMetaInfo.Id;
        }
        return null;
    }
}