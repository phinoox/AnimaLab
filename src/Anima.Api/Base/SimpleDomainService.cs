using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.Base;

public abstract class SimpleDomainService<TEntity> : DomainService, IDomainService<TEntity> 
    where TEntity : class
{
    protected SimpleDomainService(ICoreServicesProvider coreServices, ILogger logger) 
        : base(coreServices, logger) { }

    protected abstract DbContext GetDbContext();

    public abstract Task<ApiResponseDto<IdentityDto>> CreateAsync(TEntity entity);

    public async Task<ApiResponseDto<TEntity>> UpdateAsync(Guid id, TEntity updateDto)
    {
        try
        {
            var responseDto = await OnUpdateAsync(id, updateDto);
            if(!responseDto.Successful)
                _logger.LogError("Error during update of {EntityType} with id {entityId}", typeof(TEntity).Name, id);
            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during update of {EntityType}", typeof(TEntity).Name);
            return ApiResponseDto<TEntity>.ServerError("An error occurred during update.");
        }
    }

    public async Task<ApiResponseDto<IdentityDto>> DeleteAsync(Guid id)
    {
        try
        {
            var responseDto = await OnDeleteAsync(id);
             if(!responseDto.Successful)
                _logger.LogError("Error during delete of {EntityType} with id {entityId}", typeof(TEntity).Name, id);
            return responseDto;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during deletion of {EntityType}", typeof(TEntity).Name);
            return ApiResponseDto<IdentityDto>.ServerError("An error occurred during deletion.");
        }
    }

    public abstract Task<ApiResponseDto<TEntity>> GetAsync(Guid id);

    protected abstract Task<ApiResponseDto<TEntity>> OnUpdateAsync(Guid id, TEntity updateDto);
    protected abstract Task<ApiResponseDto<IdentityDto>> OnDeleteAsync(Guid id);
}