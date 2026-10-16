using Anima.Api.ComponentServices.Services;
using Anima.Core.Dtos.Shared;
using Microsoft.AspNetCore.Mvc;

namespace Anima.Api.Infrastructure.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SearchController : ControllerBase
{
    private readonly SearchOrchestrator _orchestrator;

    public SearchController(SearchOrchestrator orchestrator)
    {
        _orchestrator = orchestrator;
    }

    /// <summary>
    /// Performs a search across all registered providers or filters by a specific type.
    /// </summary>
    /// <param name="request">The search request containing query, optional type, projectId, and pagination.</param>
    /// <returns>A paged list of search hits.</returns>
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] SearchRequestDto request)
    {
        if (string.IsNullOrWhiteSpace(request.Query))
            return BadRequest("Search query cannot be empty.");

        var results = await _orchestrator.GlobalSearchAsync(request);
        return Ok(results);
    }
}
