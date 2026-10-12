namespace Anima.Api.ComponentServices.Interfaces;

using Anima.Core.Dtos.Shared;

public interface ITagService
{
    Task<ApiResponseDto<MetaIdentityResponseDto>> AddTagAsync(Guid projectId, Guid targetId, string name);
    Task<ApiResponseDto<MetaIdentityResponseDto>> RemoveTagAsync(Guid projectId,Guid targetId, Guid tagId);
}
