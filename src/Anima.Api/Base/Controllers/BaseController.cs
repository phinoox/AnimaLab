using Anima.Api.Base.Interfaces;
using Anima.Core.Dtos.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Anima.Api.Base.Controllers;

/// <summary>
/// Provides a foundational base class for all API controllers, 
/// implementing standard CRUD operations and response mapping.
/// </summary>
/// <typeparam name="TService">The type of the domain service responsible for entity logic.</typeparam>
/// <typeparam name="TEntity">The type of the entity being managed.</typeparam>
[ApiController]
[Route("api/[controller]")]
public abstract class BaseController<TService, TEntity> : ControllerBase 
    where TService : IDomainService<TEntity>
    where TEntity : class
{
    protected readonly TService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="BaseController{TService, TEntity}"/> class.
    /// </summary>
    /// <param name="service">The domain service instance.</param>
    protected BaseController(TService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retrieves an entity by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the entity.</param>
    /// <returns>An <see cref="IActionResult"/> containing the entity data or an error response.</returns>
    [HttpGet("{id}")]
    public virtual async Task<IActionResult> GetAsync(Guid id)
    {
        var result = await _service.GetAsync(id);
        return MapApiResponse(result);
    }

    /// <summary>
    /// Creates a new entity.
    /// </summary>
    /// <param name="entity">The entity data to create.</param>
    /// <returns>An <see cref="IActionResult"/> containing the created entity's identity or an error response.</returns>
    [HttpPost]
    public virtual async Task<IActionResult> CreateAsync([FromBody] TEntity entity)
    {
        var result = await _service.CreateAsync(entity);
        return MapApiResponse(result);
    }

    /// <summary>
    /// Updates an existing entity.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to update.</param>
    /// <param name="updateDto">The updated data for the entity.</param>
    /// <returns>An <see cref="IActionResult"/> indicating success or failure.</returns>
    [HttpPut("{id}")]
    public virtual async Task<IActionResult> UpdateAsync(Guid id, [FromBody] TEntity updateDto)
    {
        var result = await _service.UpdateAsync(id, updateDto);
        return MapApiResponse(result);
    }

    /// <summary>
    /// Deletes an existing entity.
    /// </summary>
    /// <param name="id">The unique identifier of the entity to delete.</param>
    /// <returns>An <see cref="IActionResult"/> indicating success or failure.</returns>
    [HttpDelete("{id}")]
    public virtual async Task<IActionResult> DeleteAsync(Guid id)
    {
        var result = await _service.DeleteAsync(id);
        return MapApiResponse(result);
    }

    /// <summary>
    /// Maps an <see cref="ApiResponseDto{T}"/> to the appropriate HTTP response.
    /// </summary>
    /// <typeparam name="T">The type of the data in the response.</typeparam>
    /// <param name="response">The API response object.</param>
    /// <returns>An <see cref="IActionResult"/> representing the mapped HTTP status code and body.</returns>
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