using Anima.Api.Base;
using Anima.Api.ComponentServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.ContentBase;
using Anima.Core.Models.Tagging;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.ComponentServices.Services;

public class TagService : DomainService, ITagService
{
    private readonly AppDbContext _db;

    public TagService(AppDbContext db, ILogger<TagService> logger, ICoreServicesProvider coreServices) 
        : base(coreServices, logger)
    {
        _db = db;
    }

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

    public async Task<ApiResponseDto<IdentityDto>> RemoveTagAsync(Guid projectId,Guid targetId, Guid tagId)
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