namespace Anima.Core.Base.MetaBase;


/// <summary>
/// Represents the absolute minimum structural requirement for an entity within the Anima ecosystem.
/// This class is intended to be used as a lightweight identifier wrapper when full meta-information 
/// (like title, slug, or timestamps) is not required for a specific context.
/// </summary>
public abstract class EntityBase
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Key]
    public Guid Id {get;init;}
}