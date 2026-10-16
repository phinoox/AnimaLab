namespace Anima.Api.Infrastructure.Controllers;

using Anima.Core.Base.Commands;
using Anima.Core.Models.Authentication;
using Anima.Api.Infrastructure.Handlers.User;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/users")]
public class UserController : ControllerBase
{
    private readonly IApiCommandBus _commandBus;

    public UserController(IApiCommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    [HttpGet("profile")]
    public async Task<IActionResult> GetProfile()
    {
        var response = await _commandBus.SendAsync<UserMetaInfo>(new GetProfileCommand(),new RequestContext());
        return response.Successful ? Ok(response) : BadRequest(response);
    }

    [HttpPatch("profile")]
    public async Task<IActionResult> UpdateProfile([FromBody] UserMetaInfo updateData)
    {
        var command = new UpdateProfileCommand(updateData);
        var response = await _commandBus.SendAsync<SuccessResponseDto>(command,new RequestContext());
        return response.Successful ? Ok(response) : BadRequest(response);
    }
}
