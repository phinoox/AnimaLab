namespace Anima.Core.Base.Commands;

using Anima.Core.Base.Commands;
using Anima.Core.Dtos.Authentication;
using Anima.Core.Models.Authentication;

public record RegisterCommand(RegisterDto Data) : ICommand;
public record SignInCommand(SignInDto Data) : ICommand;
