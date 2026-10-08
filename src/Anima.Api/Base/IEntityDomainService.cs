using Anima.Core.Dtos.Shared;

namespace Anima.Api.Base;

public interface IEntityDomainService<TMeta, TEntity>
where TMeta : BaseMetaInfo
where TEntity : MetaEntityBase<TMeta>
{
    Task<ApiResponseDto<CreateResponseDto>> CreateAsync(string projectIdentifier, TMeta metaInfo); 
    Task<ApiResponseDto<TEntity>> GetAsync(string projectIdentifier, string entityIdentifier);
    Task<ApiResponseDto<TEntity>> UpdateAsync(string projectIdentifier, string entityIdentifier, TEntity updateDto);
    Task<ApiResponseDto<TMeta>> UpdateMetaInfoAsync(string projectIdentifier, string entityIdentifier, TMeta metaDto);
    Task<ApiResponseDto<IdentityDto>> DeleteAsync(string projectIdentifier, string entityIdentifier);
    
}
