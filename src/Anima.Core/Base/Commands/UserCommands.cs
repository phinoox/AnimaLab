namespace Anima.Core.Base.Commands;

using Anima.Core.Base.Commands;
using Anima.Core.Models.Authentication;

public record GetProfileCommand() : IApiCommand;
public record UpdateProfileCommand(UserMetaInfo Data) : IApiCommand;
