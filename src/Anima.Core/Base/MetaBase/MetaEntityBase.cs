namespace Anima.Core.Base.MetaBase;

/// <summary>
/// Represents an entity that possesses a rich metadata component, establishing a "body-soul" relationship.
/// This pattern separates the core identity (Body) from its descriptive and lifecycle information (Soul/Meta).
/// </summary>
/// <typeparam name="TMeta">The specific type of <see cref="BaseMetaInfo"/> used to enrich this entity.</typeparam>
public abstract class MetaEntityBase<TMeta> : EntityBase where TMeta : BaseMetaInfo
{
    /// <summary>
    /// Gets or sets the rich metadata associated with this entity.
    /// </summary>
    [ForeignKey("Id")]
    public virtual TMeta MetaInfo { get; set; } = null!;
}