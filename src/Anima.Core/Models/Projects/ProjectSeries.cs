// =============================================================================

using System.Text.Json.Serialization;

namespace Anima.Core.Models.Projects;

/// <summary>
/// Represents a collection of related projects (e.g., a book series or game franchise).
/// A series acts as the highest-level container in the hierarchy, anchoring multiple projects.
/// </summary>
[ModelDependency(typeof(ProjectSeriesMetaInfo))]
public class ProjectSeries : MetaEntity<ProjectSeriesMetaInfo>
{
    /// <summary>
    /// Collection of projects that belong to this series.
    /// </summary>
    [JsonIgnore]
    public virtual ICollection<Project> Projects { get; set; } = new List<Project>();
}
