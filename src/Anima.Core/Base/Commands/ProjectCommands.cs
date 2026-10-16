namespace Anima.Core.Base.Commands;

using Anima.Core.Base.Commands;
using Anima.Core.Models.Projects;

public record CreateProjectCommand(ProjectMetaInfo Meta) : IApiCommand;

public record GetProjectCommand() : IApiCommand;
public record GetProjectListCommand() : IApiCommand;
public record UpdateProjectBodyCommand(Project Data) : IApiCommand;
public record UpdateProjectMetaCommand(ProjectMetaInfo Meta) : IApiCommand;
public record DeleteProjectCommand() : IApiCommand;
