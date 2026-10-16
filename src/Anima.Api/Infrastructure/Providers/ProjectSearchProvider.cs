using Anima.Api.ComponentServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Projects;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.Infrastructure.Providers;

public class ProjectSearchProvider : ISearchableProvider
{
    private readonly AppDbContext _db;

    public ProjectSearchProvider(AppDbContext db)
    {
        _db = db;
    }

    public string SearchType => typeof(Project).Name;

    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        var metaQuery = _db.ProjectMetaInfos.AsQueryable();

        if (projectId.HasValue)
        {
            metaQuery = metaQuery.Where(m => m.Id == projectId.Value);
        }

        return await metaQuery
            .Where(m => m.Title.Contains(query) || m.Slug.Contains(query))
            .Select(m => new SearchHitDto
            {
                ResourceId = m.Id,
                DisplayName = m.Title,
                Slug = m.Slug,
                ResourceType = "Project",
                ScopeId = m.Id,
                ResourceLink = $"/api/v1/projects/{m.Slug}"
            })
            .ToListAsync();
    }
}
