namespace Anima.Core.Base.Commands;

using Anima.Core.Base.Commands;
using Anima.Core.Models.Authentication;

public record GetProfileCommand() : ICommand;
public record UpdateProfileCommand(UserMetaInfo Data) : ICommand;
