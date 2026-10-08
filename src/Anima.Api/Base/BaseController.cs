using Anima.Core.Dtos.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Anima.Api.Base;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController<TService, TEntity> : ControllerBase 
    where TService : IDomainService<TEntity>
    where TEntity : class
{
    protected readonly TService _service;

    protected BaseController(TService service)
    {
        _service = service;
    }

    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetAsync(Guid id)
    {
        var result = await _service.GetAsync(id);
        return MapApiResponse(result);
    }

    [HttpPost]
    public virtual async Task<IActionResult> CreateAsync([FromBody] TEntity entity)
    {
        var result = await _service.CreateAsync(entity);
        return MapApiResponse(result);
    }

    [HttpPut("{id}")]
    public virtual async Task<IActionResult> UpdateAsync(Guid id, [FromBody] TEntity updateDto)
    {
        var result = await _service.UpdateAsync(id, updateDto);
        return MapApiResponse(result);
    }

    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return MapApiResponse(result);
    }

    protected IActionResult MapApiResponse<T>(ApiResponseDto<T> response) where T: class
    {
        return response.StatusCode switch
        {
            System.Net.HttpStatusCode.OK => Ok(response.Data),
            System.Net.HttpStatusCode.NotFound => NotFound(response.Message),
            System.Net.HttpStatusCode.Forbidden => Forbid(),
            System.Net.HttpStatusCode.Unauthorized => Unauthorized(response.Message),
            System.Net.HttpStatusCode.BadRequest => BadRequest(response.Message),
            System.Net.HttpStatusCode.Conflict => StatusCode((int)System.Net.HttpStatusCode.Conflict, response.Message),
            _ => StatusCode((int)response.StatusCode, response.Message)
        };
    }
}