namespace Anima.Api.ComponentServices.Interfaces;

using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Authentication;

public interface ICommentService
{
    Task<ApiResponseDto<IdentityDto>> AddCommentAsync(Guid projectId, Guid parentId, Guid targetId, string text, Guid userId);
    Task<ApiResponseDto<IdentityDto>> DeleteCommentAsync(Guid projectId,Guid commentId);
}
