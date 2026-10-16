using Anima.Api.ComponentServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Blog;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.Infrastructure.Providers;

public class BlogPostSearchProvider : ISearchableProvider
{
    private readonly AppDbContext _db;

    public BlogPostSearchProvider(AppDbContext db)
    {
        _db = db;
    }

    public string SearchType => typeof(BlogPost).Name; // Returns "BlogPost"

    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        var metaQuery = _db.ContentMetaInfos.AsQueryable();

        // 1. Filter by TypeHint to ensure we only get BlogPosts
        metaQuery = metaQuery.Where(m => m.TypeHint == "BlogPost");

        // 2. If projectId is provided, filter by the parent (Project)
        if (projectId.HasValue)
        {
            metaQuery = metaQuery.Where(m => m.ParentId == projectId.Value);
        }

        return await metaQuery
            .Where(m => m.Title.Contains(query) || m.Slug.Contains(query))
            .Select(m => new SearchHitDto
            {
                ResourceId = m.Id,
                DisplayName = m.Title,
                Slug = m.Slug,
                ResourceType = "BlogPost",
                ScopeId = m.ParentId, // The project it belongs to
                ResourceLink = $"/api/v1/blog-posts/{m.Slug}"
            })
            .ToListAsync();
    }
}
