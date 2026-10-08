using Anima.Api.Base;
using Anima.Api.ComponentServices.Interfaces;
using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Authentication;
using Anima.Core.Models.ContentBase;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.ComponentServices.Services;

/// <summary>
/// Provides specialized logic for managing comments within a project's content.
/// </summary>
public class CommentService : DomainService, ICommentService
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="CommentService"/> class.
    /// </summary>
    /// <param name="db">The application database context.</param>
    /// <param name="logger">The logger for recording service-level events and errors.</param>
    /// <param name="coreServices">The provider for accessing cross-cutting core services.</param>
    public CommentService(AppDbContext db, ILogger<CommentService> logger, ICoreServicesProvider coreServices) 
        : base(coreServices, logger)
    {
        _db = db;
    }

    /// <summary>
    /// Adds a new comment to a target content item.
    /// </summary>
    /// <param name="projectId">The ID of the project containing the content.</param>
    /// <param name="parentid">The ID of the parent comment, if this is a reply; otherwise, null.</param>
    /// <param name="targetId">The unique identifier of the content being commented on.</param>
    /// <param name="text">The text content of the comment.</param>
    /// <param name="userId">The ID of the user posting the comment.</param>
    /// <returns>An <see cref="ApiResponseDto{IdentityDto}"/> containing the new comment's ID on success, or an error response.</returns>
    public async Task<ApiResponseDto<IdentityDto>> AddCommentAsync(Guid projectId, Guid parentid, Guid targetId, string text, Guid userId)
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

    /// <summary>
    /// Deletes an existing comment.
    /// </summary>
    /// <param name="projectId">The ID of the project containing the comment.</param>
    /// <param name="commentId">The unique identifier of the comment to delete.</param>
    /// <returns>An <see cref="ApiResponseDto{IdentityDto}"/> indicating success or failure.</returns>
    public async Task<ApiResponseDto<IdentityDto>> DeleteCommentAsync(Guid projectId, Guid commentId)
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