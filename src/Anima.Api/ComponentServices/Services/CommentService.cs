using Anima.Api.Base;
using Anima.Api.ComponentServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Authentication;
using Anima.Core.Models.ContentBase;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.ComponentServices.Services;

public class CommentService : DomainService, ICommentService
{
    private readonly AppDbContext _db;

    public CommentService(AppDbContext db, ILogger<CommentService> logger, ICoreServicesProvider coreServices) 
        : base(coreServices, logger)
    {
        _db = db;
    }

    public async Task<ApiResponseDto<IdentityDto>> AddCommentAsync(Guid projectId,Guid parentid, Guid targetId, string text, Guid userId)
    {
        // 1. Permission Check (Can the user edit this content?)
        var authorizationError = await CheckAccessAsync<IdentityDto>(projectId, PermissionsEnum.CanEdit);
        if (authorizationError != null) 
            return authorizationError;
        try
        {
            // 2. Find the target anchor and check settings
            var content = await _db.ContentMetaInfos
                .Include(c => c.Comments)
                .FirstOrDefaultAsync(c => c.Id == targetId);

            if (content == null) return ApiResponseDto<IdentityDto>.NotFound("Target content not found.");

            // 3. Capability Check: Are comments enabled?
            if (content.CommentSettings == CommentSettingsEnum.Disabled)
            {
                return ApiResponseDto<IdentityDto>.Forbidden("Comments are disabled for this content.");
            }

            // 4. Create the comment
            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Text = text,
                ParentCommentId = parentid,
                CreatedAt = DateTime.UtcNow
            };

            _db.Comments.Add(comment);
            await _db.SaveChangesAsync();

            return ApiResponseDto<IdentityDto>.Success(new IdentityDto(){Id=comment.Id});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error adding comment to {TargetId}", targetId);
            return ApiResponseDto<IdentityDto>.ServerError("An error occurred while posting a comment.");
        }
    }

    public async Task<ApiResponseDto<IdentityDto>> DeleteCommentAsync(Guid projectId,Guid commentId)
    {
        var authorizationError = await CheckAccessAsync<IdentityDto>(projectId, PermissionsEnum.CanEdit);
        if (authorizationError != null) 
            return authorizationError;
        try
        {
            var comment = await _db.Comments.FindAsync(commentId);
            if (comment == null) return ApiResponseDto<IdentityDto>.NotFound("Comment not found.");

            _db.Comments.Remove(comment);
            await _db.SaveChangesAsync();

            return ApiResponseDto<IdentityDto>.Success(new IdentityDto(){Id=commentId});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting comment {CommentId}", commentId);
            return ApiResponseDto<IdentityDto>.ServerError("An error occurred while deleting the comment.");
        }
    }
}