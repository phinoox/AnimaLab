namespace Anima.Api.Infrastructure.Controllers;

using Anima.Core.Base.Commands;
using Anima.Core.Models.Blog;
using Anima.Core.Models.ContentBase;
using Anima.Core.Dtos.Shared;
using Anima.Api.Infrastructure.Handlers.Blog;
using Microsoft.AspNetCore.Mvc;
using Anima.Api.CoreServices.Interfaces;

[ApiController]
[Route("api/v1/projects/{projectSlug}/blogs")]
public class BlogController : BaseController
{
    public BlogController(ISlugResolverService slug, ICommandBus bus) : base(slug, bus) { }

    [HttpPost("meta")]
    public async Task<IActionResult> CreateMeta([FromRoute] string projectSlug, [FromBody] ContentMetaInfo meta)
    {
        var projectId = await ResolveProjectAsync(projectSlug);
        if (projectId == null) return NotFound(ApiResponseDto<SuccessResponseDto>.NotFound($"Could not find project {projectSlug}"));

        var command = new CreateBlogCommand(meta);
        var context = new RequestContext(ProjectId: projectId);
        var response = await _commandBus.SendAsync<MetaIdentityResponseDto>(command, context);
        return response.Successful ? Ok(response) : BadRequest(response);
    }

    [HttpGet("{blogSlug}")]
    public async Task<IActionResult> Get([FromRoute] string projectSlug, [FromRoute] string blogSlug)
    {
        var projectId = await ResolveProjectAsync(projectSlug);
        if (projectId == null) return NotFound();

        var blogId = await ResolveScopeAsync<ContentMetaInfo>(projectId.Value, blogSlug);
        if (blogId == null) return NotFound();

        var command = new GetBlogCommand();
        // IdentityReq: MustBeAuthenticated, ScopeReq: Entity (handled by BaseCommandHandler)
        var context = new RequestContext(ProjectId: projectId.Value, EntityId: blogId);

        var response = await _commandBus.SendAsync<ContentMetaInfo>(command, context);
        return response.Successful ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{blogSlug}/body")]
    public async Task<IActionResult> UpdateBody([FromRoute] string projectSlug, [FromRoute] string blogSlug, [FromBody] BlogPost data)
    {
        var projectId = await ResolveProjectAsync(projectSlug);
        if (projectId == null) return NotFound(ApiResponseDto<SuccessResponseDto>.NotFound($"Could not find project {projectSlug}"));

        var blogId = await ResolveScopeAsync<ContentMetaInfo>(projectId.Value, blogSlug);
        if (blogId == null) return NotFound(ApiResponseDto<SuccessResponseDto>.NotFound($"Could not find project {projectSlug}"));


        var command = new UpdateBlogBodyCommand(data);
        var context = new RequestContext(ProjectId: projectId, EntityId: blogId);
        var response = await _commandBus.SendAsync<SuccessResponseDto>(command, context);
        return response.Successful ? Ok(response) : BadRequest(response);
    }

    [HttpPut("{blogSlug}/meta")]
    public async Task<IActionResult> UpdateMeta([FromRoute] string projectSlug, [FromRoute] string blogSlug, [FromBody] ContentMetaInfo meta)
    {
        var projectId = await ResolveProjectAsync(projectSlug);
        if (projectId == null) return NotFound(ApiResponseDto<SuccessResponseDto>.NotFound($"Could not find project {projectSlug}"));

        var blogId = await ResolveScopeAsync<ContentMetaInfo>(projectId.Value, blogSlug);
        if (blogId == null) return NotFound(ApiResponseDto<SuccessResponseDto>.NotFound($"Could not find project {projectSlug}"));


        var command = new UpdateBlogMetaCommand(meta);
        var context = new RequestContext(ProjectId: projectId, EntityId: blogId);

        var response = await _commandBus.SendAsync<SuccessResponseDto>(command, context);
        return response.Successful ? Ok(response) : BadRequest(response);
    }

    [HttpDelete("{blogSlug}")]
    public async Task<IActionResult> Delete([FromRoute] string projectSlug, [FromRoute] string blogSlug)
    {
        var projectId = await ResolveProjectAsync(projectSlug);
        if (projectId == null) return NotFound(ApiResponseDto<SuccessResponseDto>.NotFound($"Could not find project {projectSlug}"));

        var blogId = await ResolveScopeAsync<ContentMetaInfo>(projectId.Value, blogSlug);
        if (blogId == null) return NotFound(ApiResponseDto<SuccessResponseDto>.NotFound($"Could not find project {projectSlug}"));

        var command = new DeleteBlogCommand();
        var context = new RequestContext(ProjectId: projectId, EntityId: blogId);
        var response = await _commandBus.SendAsync<SuccessResponseDto>(command, context);
        return response.Successful ? Ok(response) : BadRequest(response);
    }
}
