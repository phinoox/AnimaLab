namespace Anima.Core.Base.MetaBase;

/// <summary>
/// Represents metadata that is scoped within the context of a specific project.
/// Entities using this model are inherently tied to a parent project.
/// </summary>
public abstract class ProjectScopedMetaInfo : BaseMetaInfo
{
    /// <summary>
    /// Gets or sets the identifier of the project this meta-information belongs to.
    /// </summary>
    public Guid ProjectId { get; init; }
}