namespace Anima.Api.Infrastructure.Controllers;

using Anima.Core.Base.Commands;
using Anima.Core.Models.Projects;
using Anima.Core.Dtos.Shared;
using Anima.Api.Infrastructure.Handlers.Projects;
using Microsoft.AspNetCore.Mvc;
using Anima.Api.CoreServices.Interfaces;

[ApiController]
[Route("api/v1/projects")]
public class ProjectController : BaseController
{
    public ProjectController(ISlugResolverService slug, ICommandBus bus) : base(slug, bus) { }

    [HttpPost("meta")]
    public async Task<IActionResult> CreateMeta([FromBody] ProjectMetaInfo meta)
    {
        var command = new CreateProjectCommand(meta);

        var response = await _commandBus.SendAsync<MetaIdentityResponseDto>(command, new RequestContext());
        return response.Successful ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{projectSlug}")]
    public async Task<IActionResult> Get([FromRoute] string projectSlug)
    {
         var projectId = await ResolveProjectAsync(projectSlug);
        if (projectId == null) return NotFound(ApiResponseDto<SuccessResponseDto>.NotFound($"Could not find project with identifier {projectSlug}"));

        var command = new GetProjectCommand();
        // IdentityReq: MustBeAuthenticated, ScopeReq: None (handled by BaseCommandHandler)
        var response = await _commandBus.SendAsync<ProjectMetaInfo>(command, new RequestContext(ProjectId: projectId));
        return response.Successful ? Ok(response) : BadRequest(response);
    }

    [HttpGet()]
    public async Task<IActionResult> Get()
    {
        var command = new GetProjectListCommand();
        // IdentityReq: MustBeAuthenticated, ScopeReq: None (handled by BaseCommandHandler)
        var response = await _commandBus.SendAsync<IEnumerable<ProjectMetaInfo>>(command, new RequestContext());
        return response.Successful ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{projectSlug}/body")]
    public async Task<IActionResult> UpdateBody([FromRoute] string projectSlug, [FromBody] Project data)
    {
        var projectId = await ResolveProjectAsync(projectSlug);
        if (projectId == null) return NotFound(ApiResponseDto<SuccessResponseDto>.NotFound($"Could not find project with identifier {projectSlug}"));

        var command = new UpdateProjectBodyCommand(data);
        var context = new RequestContext(ProjectId: projectId);
        var response = await _commandBus.SendAsync<SuccessResponseDto>(command, context);
        return response.Successful ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{projectSlug}/meta")]
    public async Task<IActionResult> UpdateMeta([FromRoute] string projectSlug, [FromBody] ProjectMetaInfo meta)
    {
        var projectId = await ResolveProjectAsync(projectSlug);
        if (projectId == null) return NotFound(ApiResponseDto<SuccessResponseDto>.NotFound($"Could not find project with identifier {projectSlug}"));

        var command = new UpdateProjectMetaCommand(meta);
        var context = new RequestContext(ProjectId: projectId);
        var response = await _commandBus.SendAsync<SuccessResponseDto>(command, context);
        return response.Successful ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{slug}")]
    public async Task<IActionResult> Delete([FromRoute] string slug)
    {
        var projectId = await ResolveProjectAsync(slug);
        if (projectId == null) return NotFound(ApiResponseDto<SuccessResponseDto>.NotFound($"Could not find project with identifier {slug}"));

        var command = new DeleteProjectCommand();
        var context = new RequestContext(ProjectId: projectId);
        var response = await _commandBus.SendAsync<SuccessResponseDto>(command, context);
        return response.Successful ? Ok(response) : BadRequest(response);
    }

}