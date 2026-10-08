using Anima.Core.Models.Projects;
using Anima.Core.Models.Tagging;
using Anima.Core.Models.Versioning;

namespace Anima.Core.Models.ContentBase;

/// <summary>
/// Represents a content item (character, world, mechanic, etc.) in the game development system.
/// </summary>
[ModelDependency(typeof(Project))]
public class ContentMetaInfo : ProjectScopedMetaInfo
{
    // --- Relationships & Domain Data ---
   
    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!; 

    
    [Required]
    public ContentStatusEnum Status { get; set; } = ContentStatusEnum.Draft;

    [Required]
    public ViewModeEnum ViewMode { get; set; } = ViewModeEnum.PrivateWriting;

    public int Version { get; set; } = 0;
    public int OrderIndex { get; set; } = 0;

    [Required]
    public CommentSettingsEnum CommentSettings { get; set; } = CommentSettingsEnum.Disabled;

    [MaxLength(4096)]
    public string? References { get; set; }

    
    // Collection navigation properties
    public virtual ICollection<ContentVersionLog> VersionLogs { get; set; } = new List<ContentVersionLog>();
    public virtual ICollection<AssetLink> AssetLinks { get; set; } = new List<AssetLink>();
    public virtual ICollection<MediaAttachment> MediaAttachments { get; set; } = new List<MediaAttachment>();
    public virtual ReviewStatus ReviewStatus { get; set; } = null!; 

    public virtual ICollection<Comment> Comments {get;set;} = new List<Comment>();
    public virtual ICollection<ContentTagRelation> MetaInfoTagRelations { get; set; } = new List<ContentTagRelation>();
}
