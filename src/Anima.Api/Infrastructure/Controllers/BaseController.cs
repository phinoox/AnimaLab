namespace Anima.Api.Infrastructure.Controllers;

using Anima.Core.Base.Commands;
using Anima.Core.Models.Projects;
using Anima.Core.Dtos.Shared;
using Anima.Api.Infrastructure.Handlers.Projects;
using Microsoft.AspNetCore.Mvc;
using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Base.MetaBase;

[ApiController]
[Route("api/v1/projects/{projectSlug}")]
public abstract class BaseController : ControllerBase
{
    protected readonly ISlugResolverService _slugResolver;
    protected readonly ICommandBus _commandBus;

    protected BaseController(ISlugResolverService slugResolver, ICommandBus commandBus)
    {
        _slugResolver = slugResolver;
        _commandBus = commandBus;
    }

    protected async Task<Guid?> ResolveProjectAsync(string slug)
    {
        var id = await _slugResolver.ResolveSlugAsync<ProjectMetaInfo>(slug);
            
        return id;
    }

    protected async Task<Guid?> ResolveScopeAsync<TMeta>(Guid projectId,string slug) where TMeta : ScopedMetaInfo
    {
        var id = await _slugResolver.ResolveSlugAsync<TMeta>(projectId,slug);
            
        return id;
    }
}
