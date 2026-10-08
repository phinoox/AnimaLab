public interface ISlugResolverService
{
    Task<Guid?> ResolveSlugAsync<TMeta>(string identifier) where TMeta : BaseMetaInfo;
    Task<Guid?> ResolveSlugAsync<TMeta>(Guid projectId, string identifier) where TMeta : ProjectScopedMetaInfo;
}
