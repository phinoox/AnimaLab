using Anima.Api.Base;
using Anima.Api.ComponentServices.Interfaces;
using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.ContentBase;
using Anima.Core.Models.Projects;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.ComponentServices.Services;

/// <summary>
/// Provides specialized logic for managing the review status of content items.
/// </summary>
public class ReviewStatusService : DomainService, IReviewStatusService
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="ReviewStatusService"/> class.
    /// </summary>
    /// <param name="db">The application database context.</param>
    /// <param name="logger">The logger for recording service-level events and errors.</param>
    /// <param name="coreServices">The provider for accessing cross-cutting core services.</param>
    public ReviewStatusService(AppDbContext db, ILogger<ReviewStatusService> logger, ICoreServicesProvider coreServices) 
        : base(coreServices, logger)
    {
        _db = db;
    }

    /// <summary>
    /// Updates the review status of a specific content item.
    /// </summary>
    /// <param name="projectId">The ID of the project containing the content.</param>
    /// <param name="targetId">The unique identifier of the content to update.</param>
    /// <param name="statusValue">The integer value representing the new status, cast to <see cref="ReviewStatusEnum"/>.</param>
    /// <returns>An <see cref="ApiResponseDto{IdentityDto}"/> containing the target ID on success, or an error response.</returns>
    public async Task<ApiResponseDto<IdentityDto>> SetStatusAsync(Guid projectId, Guid targetId, int statusValue)
    {
        // 1. Permission Check
        var authorizationError = await CheckAccessAsync<IdentityDto>(projectId, PermissionsEnum.CanEdit);
        if (authorizationError != null) 
            return authorizationError;
       
        try
        {
            // 2. Locate the Target Anchor (Soul)
            var meta = await _db.ContentMetaInfos.FindAsync(targetId);
            if (meta == null) return ApiResponseDto<IdentityDto>.NotFound("Target content not found.");

            // 3. Perform the update on the component property
            // Casting the int to the enum as requested by the interface
            meta.ReviewStatus.Status = (ReviewStatusEnum)statusValue;

            await _db.SaveChangesAsync();

            return ApiResponseDto<IdentityDto>.Success(new IdentityDto(){Id=targetId});
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating review status for {TargetId}", targetId);
            return ApiResponseDto<IdentityDto>.ServerError("An error occurred during status update.");
        }
    }
}