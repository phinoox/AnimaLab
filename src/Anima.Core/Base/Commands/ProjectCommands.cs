namespace Anima.Core.Base.Commands;

using Anima.Core.Base.Commands;
using Anima.Core.Models.Projects;

public record CreateProjectCommand(ProjectMetaInfo Meta) : ICommand;

public record GetProjectCommand() : ICommand;
public record GetProjectListCommand() : ICommand;
public record UpdateProjectBodyCommand(Project Data) : ICommand;
public record UpdateProjectMetaCommand(ProjectMetaInfo Meta) : ICommand;
public record DeleteProjectCommand() : ICommand;
