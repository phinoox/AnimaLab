// =============================================================================

using System.Text.Json.Serialization;

namespace Anima.Core.Models.Projects;

/// <summary>
/// Represents a collection of related projects (e.g., a book series or game franchise).
/// A series acts as the highest-level container in the hierarchy, anchoring multiple projects.
/// </summary>
[ModelDependency(typeof(ProjectSeriesMetaInfo))]
public class ProjectSeries
{
    /// <summary>
    /// Unique identifier for the series.
    /// </summary>
    [Key]
    public Guid Id { get; init; }

    /// <summary>
    /// The ID of the associated identity anchor for the series (Vertical Unification).
    /// </summary>
    [ForeignKey("Id")]
    public virtual ProjectSeriesMetaInfo ProjectSeriesMetaInfo { get; set; } = null!;

    /// <summary>
    /// Collection of projects that belong to this series.
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
