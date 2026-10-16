namespace Anima.Core.Commands.Projects;

using Anima.Core.Base.Commands;
using Anima.Core.Models.Projects;

/// <summary>
/// Command to create a new Project. 
/// Includes the Soul (MetaInfo) and Body properties required for creation.
/// </summary>
public record CreateProjectCommand(ProjectMetaInfo MetaInfo, string Description, bool EnableUserRegistration, bool AllowManualInvites, string? PrimaryFormat, string? Genre, string? Theme, string? Tone, string? Audience) : IApiCommand;


/// <summary>
/// Command to create the "Soul" (MetaInfo) of a project.
/// This is the first step in the chunked creation workflow.
/// </summary>
public record CreateProjectMetaCommand(ProjectMetaInfo MetaInfo) : IApiCommand;


/// <summary>
/// Command to delete a Project by its unique identifier.
/// </summary>
public record DeleteProjectCommand(string ProjectSlug) : IApiCommand;


/// <summary>
/// Command to update the "Body" properties of an existing project.
/// </summary>
public record UpdateProjectBodyCommand(string ProjectSlug, Project BodyProperties) : IApiCommand;


/// <summary>
/// Command to update an existing Project's metadata and body properties.
/// </summary>
public record UpdateProjectCommand(Guid ProjectId, ProjectMetaInfo? MetaInfo, string? Description, bool? EnableUserRegistration, bool? AllowManualInvites, string? PrimaryFormat, string? Genre, string? Theme, string? Tone, string? Audience) : IApiCommand;


/// <summary>
/// Command to update the "Soul" (MetaInfo) of an existing project.
/// </summary>
public record UpdateProjectMetaCommand(string ProjectSlug, ProjectMetaInfo MetaInfo) : IApiCommand;