namespace Anima.Api.Base;

using Anima.Core.Dtos.Shared;

public interface IDomainService<TEntity> where TEntity : class
{
    Task<ApiResponseDto<TEntity>> GetAsync(Guid id);
    Task<ApiResponseDto<IdentityDto>> CreateAsync(TEntity entity);
    Task<ApiResponseDto<TEntity>> UpdateAsync(Guid id, TEntity updateDto);
    Task<ApiResponseDto<IdentityDto>> DeleteAsync(Guid id);
}
