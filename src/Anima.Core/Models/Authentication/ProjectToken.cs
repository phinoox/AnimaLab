using System.Text.Json.Serialization;
using Anima.Core.Models.Projects;

/// <summary>
/// Provides access credentials for a specific project context.
/// Following Law I: This is a 1:1 extension, so Id == ProjectId.
/// </summary>
public class ProjectToken
{
    [Key]
    public Guid Id { get; set; } // Same as ProjectId

    [Required]
    public Guid ProjectId { get; set; }

    [ForeignKey("ProjectId")]
    public virtual Project Project { get; set; } = null!;
    public bool IsActive { get; set; }
    public string TokenName { get; set; }
    public DateTime? ExpiresAt { get; set; }
    [JsonIgnore]
    public DateTime CreatedAt { get; set; }
    [JsonIgnore]
    public string TokenHash { get; set; }
}
