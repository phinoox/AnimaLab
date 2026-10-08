using Anima.Data.Contexts;

namespace Anima.Api.CoreServices.Services;

public class SlugResolverService
{
    private AppDbContext _db;

    public SlugResolverService(AppDbContext db)
    {
        _db=db;
    }

   
    public async Task<Guid?> ResolveSlugAsync<TMeta>(string identifier) where TMeta : BaseMetaInfo
    {
        if (Guid.TryParse(identifier, out var guid))
        {
            return guid;
        }

        var miSet = _db.Set<TMeta>();
        if(miSet == null)
            return null;
        var projectMetaInfo = miSet.FirstOrDefault(mi => mi.Slug == identifier );
        if(projectMetaInfo != null)
        {
            return (Guid?)projectMetaInfo.Id;
        }
        return null;
    }

    public async Task<Guid?> ResolveSlugAsync<TMeta>(Guid projectId,string identifier) where TMeta : ProjectScopedMetaInfo
    {
        if (Guid.TryParse(identifier, out var guid))
        {
            return guid;
        }
        
        var miSet = _db.Set<TMeta>();
        if(miSet == null)
            return null;
        var projectMetaInfo = miSet.FirstOrDefault(mi => mi.Slug == identifier && mi.ProjectId == projectId);
        if(projectMetaInfo != null)
        {
            return (Guid?)projectMetaInfo.Id;
        }
        return null;
    }
}