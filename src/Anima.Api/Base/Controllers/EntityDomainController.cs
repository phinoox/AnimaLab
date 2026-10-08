using System.Net;
using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Dtos.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Anima.Api.Base;

/// <summary>
/// Provides a foundational base class for all API controllers that manage entities with associated metadata.
/// It implements standard CRUD operations and handles mapping domain responses to HTTP results.
/// </summary>
/// <typeparam name="TService">The type of the entity domain service.</typeparam>
/// <typeparam name="TEntity">The type of the managed entity.</typeparam>
/// <typeparam name="TMeta">The type of the metadata associated with the entity.</typeparam>
[ApiController]
[Route("api/{projectIdentifier}/[controller]")]
public abstract class EntityDomainController<TService, TEntity, TMeta> : ControllerBase 
    where TService : IEntityDomainService<TMeta, TEntity>
    where TEntity : MetaEntityBase<TMeta>
    where TMeta : BaseMetaInfo
{
    protected readonly TService _service;

    /// <summary>
    /// Initializes a new instance of the <see cref="EntityDomainController{TService, TEntity, TMeta}"/> class.
    /// </summary>
    /// <param name="service">The domain service responsible for entity operations.</param>
    protected EntityDomainController(TService service)
    {
        _service = service;
    }

    /// <summary>
    /// Creates a new entity within the specified project.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="createDto">The metadata used to create the entity.</param>
    /// <returns>An <see cref="IActionResult"/> representing the result of the creation operation.</returns>
    [HttpPost]
    public async Task<IActionResult> CreateAsync(string projectIdentifier, [FromBody] TMeta createDto)
    {
        return Ok(await _service.CreateAsync(projectIdentifier, createDto));
    }

    /// <summary>
    /// Retrieves an entity by resolving its slug within the specified project.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="identifier">The slug of the target entity.</param>
    /// <returns>An <see cref="IActionResult"/> containing the found entity or an error response.</returns>
    [HttpGet("{identifier}")]
    public virtual async Task<IActionResult> GetAsync(string projectIdentifier, string identifier)
    {
        var result = await _service.GetAsync(projectIdentifier, identifier);
        return MapApiResponse(result);
    }

    /// <summary>
    /// Updates an existing entity by resolving its slug within the specified project.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="identifier">The slug of the target entity.</param>
    /// <param name="updateDto">The updated data for the entity.</param>
    /// <returns>An <see cref="IActionResult"/> indicating success or failure.</returns>
    [HttpPut("{identifier}")]
    public virtual async Task<IActionResult> UpdateAsync(string projectIdentifier, string identifier, [FromBody] TEntity updateDto)
    {
        var result = await _service.UpdateAsync(projectIdentifier, identifier, updateDto);
        return MapApiResponse(result);
    }

    /// <summary>
    /// Updates only the metadata (the "Soul") of the entity.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="entityId">The slug of the target entity.</param>
    /// <param name="metaUpdateDto">The new metadata to apply.</param>
    /// <returns>An <see cref="IActionResult"/> indicating success or failure.</returns>
    [HttpPatch("meta")]
    public virtual async Task<IActionResult> UpdateMetaAsync(string projectIdentifier, string entityId, [FromBody] TMeta metaUpdateDto)
    {
        var result = await _service.UpdateMetaInfoAsync(projectIdentifier, entityId, metaUpdateDto);
        return MapApiResponse(result);
    }

    /// <summary>
    /// Deletes an existing entity by resolving its slug within the specified project.
    /// </summary>
    /// <param name="projectIdentifier">The slug of the project.</param>
    /// <param name="identifier">The slug of the target entity.</param>
    /// <returns>An <see cref="IActionResult"/> indicating success or failure.</returns>
    [HttpDelete("{identifier}")]
    public virtual async Task<IActionResult> DeleteAsync(string projectIdentifier, string identifier)
    {
        var result = await _service.DeleteAsync(projectIdentifier, identifier);
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
            HttpStatusCode.OK => Ok(response.Data),
            HttpStatusCode.NotFound => NotFound(response.Message),
            HttpStatusCode.Forbidden => Forbid(),
            HttpStatusCode.Unauthorized => Unauthorized(response.Message),
            HttpStatusCode.BadRequest => BadRequest(response.Message),
            HttpStatusCode.Conflict => StatusCode((int)HttpStatusCode.Conflict, response.Message),
            _ => StatusCode((int)response.StatusCode, response.Message)
        };
    }
}