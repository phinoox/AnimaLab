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

    // Link back to the Project anchor
    
    /// <summary>
    /// The ID of the associated project.
    /// </summary>
    public Guid? ProjectId { get; set; }

    /// <summary>
    /// Navigation property for the parent project.
    /// </summary>
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;

    public override  string? TypeHint {get => typeof(Project).ToString();set;} 
}
