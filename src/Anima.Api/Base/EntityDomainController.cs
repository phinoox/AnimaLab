using System.Net;
using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Anima.Api.Base;

[ApiController]
[Route("api/{projectIdentifier}/[controller]")]
public abstract class EntityDomainController<TService, TEntity, TMeta> : ControllerBase 
    where TService : IEntityDomainService<TMeta, TEntity>
    where TEntity : MetaEntityBase<TMeta>
    where TMeta : BaseMetaInfo
{
    protected readonly TService _service;

    protected EntityDomainController(TService service)
    {
        _service = service;
    }

    public async Task<IActionResult> CreateAsync(string projectIdentifier, [FromBody] TMeta createDto)
    {

        return Ok(await _service.CreateAsync(projectIdentifier, createDto));
    }
  

    [HttpGet("{identifier}")]
    public virtual async Task<IActionResult> GetAsync(string projectIdentifier, string identifier)
    {
        var result = await _service.GetAsync(projectIdentifier, identifier);
        return MapApiResponse(result);
    }

    [HttpPut("{identifier}")]
    public virtual async Task<IActionResult> UpdateAsync(string projectIdentifier, string identifier, [FromBody] TEntity updateDto)
    {
        var result = await _service.UpdateAsync(projectIdentifier, identifier, updateDto);
        return MapApiResponse(result);
    }

    /// <summary>
    /// Updates only the MetaInfo (the Soul) of the entity.
    /// </summary>
    [HttpPatch("meta")]
    public virtual async Task<IActionResult> UpdateMetaAsync(string projectIdentifier, string entityId, [FromBody] TMeta metaUpdateDto)
    {
        // We call the specialized method on the service
        var result = await _service.UpdateMetaInfoAsync(projectIdentifier, entityId, metaUpdateDto);
        return MapApiResponse(result);
    }

    [HttpDelete("{identifier}")]
    public virtual async Task<IActionResult> DeleteAsync(string projectIdentifier, string identifier)
    {
        var result = await _service.DeleteAsync(projectIdentifier, identifier);
        return MapApiResponse(result);
    }

    protected IActionResult MapApiResponse<T>(ApiResponseDto<T> response) where T : class
    {
       return response.StatusCode switch
    {
        HttpStatusCode.OK => Ok(response.Data),
        HttpStatusCode.NotFound => NotFound(response.Message),
        HttpStatusCode.Forbidden => Forbid(), // or StatusCode(403, response.Message)
        HttpStatusCode.Unauthorized => Unauthorized(response.Message),
        HttpStatusCode.BadRequest => BadRequest(response.Message),
        HttpStatusCode.Conflict => StatusCode((int)HttpStatusCode.Conflict, response.Message),
        _ => StatusCode((int)response.StatusCode, response.Message)
    };
    }




}
