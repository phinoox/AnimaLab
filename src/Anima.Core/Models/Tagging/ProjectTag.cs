namespace Anima.Core.Models.Tagging;

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a tag specifically for organizing and categorizing projects.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class ProjectTag
{
    /// <summary>
    /// Unique identifier for the project tag.
    /// </summary>
    [Key]
    public Guid Id { get; init; }

    /// <summary>
    /// The human-readable name of the tag (e.g., "High Fantasy", "Completed").
    /// </summary>
    [Required, MaxLength(128)]
    public string Name { get; set; } = "";

    /// <summary>
    /// A unique, URL-friendly slug for the tag.
    /// </summary>
    [MaxLength(128)]
    public string Slug { get; set; } = "";

    /// <summary>
    /// An optional hex color code used for visual representation in UI.
    /// </summary>
    [MaxLength(36)]
    public string? ColorHex { get; set; }

    /// <summary>
    /// Collection of relations linking this tag to various projects.
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<ProjectTagRelation> ProjectTags { get; set; } = new List<ProjectTagRelation>();
}
