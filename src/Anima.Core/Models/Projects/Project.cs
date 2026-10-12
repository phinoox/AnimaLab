// =============================================================================
using System.Text.Json.Serialization;
using Anima.Core.Models.Authentication;

namespace Anima.Core.Models.Projects;

/// <summary>
/// Represents a project in the game development management system.
/// Owned by a single User (CreatedBy).
/// </summary>
[ModelDependency(typeof(User), typeof(ProjectMetaInfo))] 
public class Project : MetaEntityBase<ProjectMetaInfo>
{
    // --- Domain Data ---
    [MaxLength(4096)]
    public string? Description { get; set; } = null!;

    public bool IsActive { get; set; } = true;
    
    
    public Guid? ProjectSeriesId { get; set; }

    [ForeignKey("ProjectSeriesId")]
    public virtual ProjectSeries? ProjectSeries { get; set; }

   

    // --- Domain Specific Metadata ---
    public bool EnableUserRegistration { get; set; } = false;
    public bool AllowManualInvites { get; set; } = true;

    [EnumDataType(typeof(PrimaryFormatEnum)), Required]
    public PrimaryFormatEnum PrimaryFormat { get; set; } = PrimaryFormatEnum.Book;

    

    [JsonIgnore]
    public ICollection<ProjectMember> Members { get; set; } = new List<ProjectMember>();

   
}
