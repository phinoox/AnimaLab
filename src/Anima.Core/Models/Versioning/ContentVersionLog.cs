using Anima.Core.Models.ContentBase;

namespace Anima.Core.Models.Versioning;

/// <summary>
/// Represents a record of a specific version change for a content item, used to support rollback capabilities.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class ContentVersionLog
{
    /// <summary>
    /// Gets or sets the unique identifier for this version log entry.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();
    
    /// <summary>
    /// Gets or sets the identifier of the associated content item.
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Gets or sets the associated content meta information entity.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; }
    /// <summary>
    /// Gets or sets the identifier of the user who performed this versioned change.
    /// </summary>
    [Required]
    public Guid ChangedByUserId { get; set; }
    
    /// <summary>
    ///
    /// Gets or sets an optional description explaining what was changed in this version.
    /// </summary>
    [MaxLength(2048)]
    public string? ChangeDescription { get; set; }
    
    /// <summary>
    /// Gets or sets the sequential version number of the content item.
    /// </summary>
    public int VersionNumber { get; set; }
    
    /// <summary>
    /// Gets or sets the timestamp when this version was recorded.
    /// </summary>
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the identifier of the user who created this log entry.
    /// </summary>
    public Guid CreatedByUserId { get; set; }
}

