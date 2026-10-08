namespace Anima.Api.ComponentServices.Interfaces;

using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Authentication;

/// <summary>
/// Defines a contract for managing comments within the project scope.
/// </summary>
public interface ICommentService
{
    /// <summary>
    /// Adds a new comment to a target content item.
    /// </summary>
    /// <param name="projectId">The ID of the project containing the content.</param>
    /// <param name="parentId">The ID of the parent comment, if this is a reply; otherwise, null.</param>
    /// <param name="targetId">The unique identifier of the content being commented on.</param>
    /// <param name="text">The text content of the comment.</param>
    /// <param name="userId">The ID of the user posting the comment.</param>
    /// <returns>An <see cref="ApiResponseDto{IdentityDto}"/> containing the new comment's ID on success, or an error response.</returns>
    Task<ApiResponseDto<IdentityDto>> AddCommentAsync(Guid projectId, Guid parentId, Guid targetId, string text, Guid userId);

    /// <summary>
    /// Deletes an existing comment.
    /// </summary>
    /// <param name="projectId">The ID of the project containing the comment.</param>
    /// <param name="commentId">The unique identifier of the comment to delete.</param>
    /// <returns>An <see cref="ApiResponseDto{IdentityDto}"/> indicating success or failure.</returns>
    Task<ApiResponseDto<IdentityDto>> DeleteCommentAsync(Guid projectId, Guid commentId);
}