using System.Text.Json.Serialization;
using Anima.Core.Models.ContentBase;

namespace Anima.Core.Models.Projects;

/// <summary>
/// Serves as the identity anchor for a Project, holding its lifecycle status and visibility settings.
/// This entity acts as the root metadata provider for all content within a specific project.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ProjectMetaInfo : BaseMetaInfo,ISoftDeletable
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


    public override  string? TypeHint {get => typeof(Project).ToString();init;}

    [MaxLength(128)]
    public string? Genre { get; set; }

    [MaxLength(128)]
    public string? Theme { get; set; }

    [EnumDataType(typeof(ToneEnum))]
    public ToneEnum Tone { get; set; } = ToneEnum.Neutral;

    [EnumDataType(typeof(AudienceEnum))]
    public AudienceEnum Audience { get; set; } = AudienceEnum.AllAges;

     [JsonIgnore]
    public bool IsDeleted { get; set; } = false;

    [JsonIgnore]
    public DateTime? DeletedAt { get; set; }
}
