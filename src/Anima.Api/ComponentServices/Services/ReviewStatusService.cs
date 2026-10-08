using Anima.Api.Base;
using Anima.Api.ComponentServices.Interfaces;
using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.ContentBase;
using Anima.Core.Models.Projects;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.ComponentServices.Services;

public class ReviewStatusService : DomainService, IReviewStatusService
{
    private readonly AppDbContext _db;

    public ReviewStatusService(AppDbContext db, ILogger<ReviewStatusService> logger, ICoreServicesProvider coreServices) 
        : base(coreServices, logger)
    {
        _db = db;
    }

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