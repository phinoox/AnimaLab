using Anima.Core.Models.ContentBase;

namespace Anima.Core.Models.Projects;

/// <summary>
/// Serves as the identity anchor for a Project, holding its lifecycle status and visibility settings.
/// This entity acts as the root metadata provider for all content within a specific project.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ProjectMetaInfo : BaseMetaInfo
{
    /// <summary>
    /// The current lifecycle stage of the project (e.g., Draft, Published).
    /// </summary>
    [Required]
    public ProjectStatusEnum Status { get; set; } = ProjectStatusEnum.Draft;
    
    /// <summary>
    /// The visibility setting for the project and its contents (e.g., Private Writing, Public).
    /// </summary>
    [Required]
    public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;


    public override  string? TypeHint {get => typeof(Project).ToString();set;} 
}
