namespace Anima.Api.ComponentServices.Interfaces;

using Anima.Core.Dtos.Shared;

/// <summary>
/// Defines a contract for managing the review status of content items.
/// </summary>
public interface IReviewStatusService
{
    /// <summary>
    /// Updates the review status of a specific content item.
    /// </summary>
    /// <param name="projectId">The ID of the project containing the content.</param>
    /// <param name="targetId">The unique identifier of the content to update.</param>
    /// <param name="statusValue">The integer value representing the new status, cast to <see cref="Anima.Core.Models.ContentBase.ReviewStatusEnum"/>.</param>
    /// <returns>An <see cref="ApiResponseDto{IdentityDto}"/> containing the target ID on success, or an error response.</returns>
    Task<ApiResponseDto<IdentityDto>> SetStatusAsync(Guid projectId, Guid targetId, int statusValue);
}