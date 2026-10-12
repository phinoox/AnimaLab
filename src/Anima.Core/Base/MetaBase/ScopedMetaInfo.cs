namespace Anima.Core.Base.MetaBase;

/// <summary>
/// Represents metadata that lives within the context of a specific scope like a project.
/// Entities using this model are inherently tied to a parent project.
/// </summary>
public abstract class ScopedMetaInfo : BaseMetaInfo
{
    /// <summary>
    /// Gets or sets the identifier of the parent this meta-information belongs to, for example a project.
    /// </summary>
    public Guid ParentId { get; init; }
}