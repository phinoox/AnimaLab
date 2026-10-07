namespace Anima.Core.Models.Tagging;

/// <summary>
/// Represents a generic junction between an entity anchored by <see cref="BaseMetaInfo"/> and a <see cref="MetaTag"/>.
/// This base class provides the structural foundation for all relationship types within the tagging system.
/// </summary>
/// <typeparam name="T">The type of content entity being tagged, which must inherit from <see cref="BaseMetaInfo"/>.</typeparam>
[DependencyIgnore]
public class TagRelation<T> where T: BaseMetaInfo
{
    /// <summary>
    /// The ID of the associated content entity.
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// The ID of the tag being associated with the entity.
    /// </summary>
    [Required]
    public Guid TagId { get; set; }

    /// <summary>
    /// Navigation property for the content entity.
    /// </summary>
    // Navigation properties
    [ForeignKey("MetaInfoId")] 
    public virtual T MetaInfo { get; set; } = null!;

    /// <summary>
    /// Navigation property for the associated tag.
    /// </summary>
    [ForeignKey("TagId")]
    public virtual MetaTag Tag { get; set; } = null!;
}

