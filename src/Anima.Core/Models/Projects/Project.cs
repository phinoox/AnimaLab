// =============================================================================
using System.Text.Json.Serialization;
using Anima.Core.Models.Authentication;

namespace Anima.Core.Models.Projects;

/// <summary>
/// Represents a project in the game development management system.
/// Owned by a single User (CreatedBy).
/// </summary>
[ModelDependency(typeof(User), typeof(ProjectMetaInfo))] 
public class Project : MetaEntity<ProjectMetaInfo>,ISoftDeletable
{
    // --- Domain Data ---
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;

    public bool IsActive { get; set; } = true;

    // --- Relationships ---
    /// <summary>
    /// Gets or sets the owner of the project.
    /// </summary>
    public Guid UserId { get; set; }
    
    [ForeignKey("UserId")]
    public virtual User User { get; set; } = null!;

    public Guid? ProjectSeriesId { get; set; }

    [ForeignKey("ProjectSeriesId")]
    public virtual ProjectSeries? ProjectSeries { get; set; }

   

    // --- Domain Specific Metadata ---
    public bool EnableUserRegistration { get; set; } = false;
    public bool AllowManualInvites { get; set; } = true;

    [EnumDataType(typeof(PrimaryFormatEnum)), Required]
    public PrimaryFormatEnum PrimaryFormat { get; set; } = PrimaryFormatEnum.Book;

    [MaxLength(128)]
    public string? Genre { get; set; }

    [MaxLength(128)]
    public string? Theme { get; set; }

    [EnumDataType(typeof(ToneEnum))]
    public ToneEnum Tone { get; set; } = ToneEnum.Neutral;

    [EnumDataType(typeof(AudienceEnum))]
    public AudienceEnum Audience { get; set; } = AudienceEnum.AllAges;

    [JsonIgnore]
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();

    [JsonIgnore]
    public bool IsDeleted { get; set; } = false;

    [JsonIgnore]
    public DateTime? DeletedAt { get; set; }
}
