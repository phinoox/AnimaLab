namespace Anima.Api.Infrastructure.Controllers;

using Anima.Core.Base.Commands;
using Anima.Core.Dtos.Authentication;
using Anima.Api.Infrastructure.Handlers.Authentication;
using Microsoft.AspNetCore.Mvc;

[ApiController]
[Route("api/v1/auth")]
public class AuthController : ControllerBase
{
    private readonly IApiCommandBus _commandBus;

    public AuthController(IApiCommandBus commandBus)
    {
        _commandBus = commandBus;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
    {
        var command = new RegisterCommand(registerDto);
        var response = await _commandBus.SendAsync<MetaIdentityResponseDto>(command, new RequestContext());
        return response.Successful ? Ok(response) : BadRequest(response);
    }

    [HttpPost("signin")]
    public async Task<IActionResult> SignIn([FromBody] SignInDto signInDto)
    {
        var command = new SignInCommand(signInDto);
        var response = await _commandBus.SendAsync<AuthResponseDto>(command,new RequestContext());
        return response.Successful ? Ok(response) : Unauthorized(response);
    }
}
