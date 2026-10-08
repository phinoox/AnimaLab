namespace Anima.Api.ComponentServices.Interfaces;

using Anima.Core.Dtos.Shared;

public interface ITagService
{
    Task<ApiResponseDto<IdentityDto>> AddTagAsync(Guid projectId, Guid targetId, string name);
    Task<ApiResponseDto<IdentityDto>> RemoveTagAsync(Guid projectId,Guid targetId, Guid tagId);
}
