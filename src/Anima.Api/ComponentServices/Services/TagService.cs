using Anima.Api.Base;
using Anima.Api.Base.Services;
using Anima.Api.ComponentServices.Interfaces;
using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.ContentBase;
using Anima.Core.Models.Tagging;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.ComponentServices.Services;

/// <summary>
/// Provides specialized logic for managing the tagging of content items within a project.
/// </summary>
public class TagService : DomainService, ITagService
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="TagService"/> class.
    /// </summary>
    /// <param name="db">The application database context.</param>
    /// <param name="logger">The logger for recording service-level events and errors.</param>
    /// <param name="coreServices">The provider for accessing cross-cutting core services.</param>
    public TagService(AppDbContext db, ILogger<TagService> logger, ICoreServicesProvider coreServices) 
        : base(coreServices, logger)
    {
        _db = db;
    }

    /// <summary>
    /// Adds a tag to a target content item. If the tag does not exist, it is created first.
    /// </summary>
    /// <param name="projectId">The ID of the project containing the content.</param>
    /// <param name="targetId">The unique identifier of the content being tagged.</param>
    /// <param name="name">The name of the tag to be applied.</param>
    /// <returns>An <see cref="ApiResponseDto{IdentityDto}"/> containing the target ID on success, or an error response.</returns>
    public async Task<ApiResponseDto<IdentityDto>> AddTagAsync(Guid projectId, Guid targetId, string name)
    {
        // 1. Permission Check (Can we edit this project scope?)
       var authorizationError = await CheckAccessAsync<IdentityDto>(projectId, PermissionsEnum.CanEdit);
        if (authorizationError != null) 
            return authorizationError;

        using var transaction = await _db.Database.BeginTransactionAsync();
        try
        {
            // 2. Find/Create the Tag (The Soul of the tag)
            var tag = await _db.MetaTags.FirstOrDefaultAsync(t => t.Name == name);
            if (tag == null)
            {
                tag = new MetaTag { Name = name };
                _db.MetaTags.Add(tag);
                await _db.SaveChangesAsync(); // Save to get the Tag Id
            }

            // 3. Find the target Content Anchor (The Soul of the content)
            var content = await _db.ContentMetaInfos.FindAsync(targetId);
            if (content == null) return ApiResponseDto<IdentityDto>.NotFound("Target content not found.");

            // 4. Check if link already exists to prevent duplicates
            bool alreadyLinked = await _db.Set<ContentTagRelation>()
                .AnyAsync(r => r.MetaInfoId == targetId && r.TagId == tag.Id);

            if (alreadyLinked) return ApiResponseDto<IdentityDto>.Success(new IdentityDto(){Id=targetId});

            // 5. Create the Junction Record (The Relationship)
            var relation = new ContentTagRelation
            {
                MetaInfoId = targetId,
                TagId = tag.Id
            };

            _db.Set<ContentTagRelation>().Add(relation);
            await _db.SaveChangesAsync();
            await transaction.CommitAsync();

            return ApiResponseDto<IdentityDto>.Success(new IdentityDto(){Id=targetId});
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            _logger.LogError(ex, "Error adding tag {TagName} to {TargetId}", name, targetId);
            return ApiResponseDto<IdentityDto>.ServerError("An error occurred while tagging content.");
        }
    }

    /// <summary>
    /// Removes a specific tag association from a target content item.
    /// </summary>
    /// <param name="projectId">The ID of the project containing the content.</param>
    /// <param name="targetId">The unique identifier of the content being untagged.</param>
    /// <param name="tagId">The unique identifier of the tag to be removed from the target.</param>
    /// <returns>An <see cref="ApiResponseDto{IdentityDto}"/> indicating success or failure.</returns>
    public async Task<ApiResponseDto<IdentityDto>> RemoveTagAsync(Guid projectId, Guid targetId, Guid tagId)
    {
        // 1. Permission Check
        var authorizationError = await CheckAccessAsync<IdentityDto>(projectId, PermissionsEnum.CanEdit);
        if (authorizationError != null) 
            return authorizationError;

        try
        {
            var relation = await _db.Set<ContentTagRelation>()
                .FirstOrDefaultAsync(r => r.MetaInfoId == targetId && r.TagId == tagId);

            if (relation == null) return ApiResponseDto<IdentityDto>.NotFound("Tag relationship not found.");

            _db.Set<ContentTagRelation>().Remove(relation);
            await _db.SaveChangesAsync();

            return ApiResponseDto<IdentityDto>.Success(new IdentityDto(){Id=targetId});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error removing tag {TagId} from {TargetId}", tagId, targetId);
            return ApiResponseDto<IdentityDto>.ServerError("An error occurred while removing the tag.");
        }
    }
}