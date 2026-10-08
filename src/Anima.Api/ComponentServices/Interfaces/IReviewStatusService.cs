namespace Anima.Api.ComponentServices.Interfaces;

using Anima.Core.Dtos.Shared;

public interface IReviewStatusService
{
    Task<ApiResponseDto<IdentityDto>> SetStatusAsync(Guid projectId, Guid targetId, int statusValue);
}
