// =============================================================================
// Anima.Core - Shared Domain Models & Interfaces
// =============================================================================

using System.Text.Json.Serialization;

namespace Anima.Core.Base.MetaBase;

/// <summary>
/// The universal identity anchor for all entities in the Anima universe.
/// Provides the core "DNA" (Identity) that remains consistent across any scope and serves as a base for specialized meta-information models.
/// </summary>
public abstract class BaseMetaInfo
{
    private string _title = "";

    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// This acts as the primary key and universal anchor for the entity's identity.
    /// </summary>
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the unique identifier of the user who created this entity.
    /// </summary>
    [Required]
    public Guid CreatedByUserId { get; init; }

    /// <summary>
    /// Gets or sets the primary display name of the entity.
    /// Updating this property automatically regenerates the associated slug.
    /// </summary>
    [Required, MaxLength(128)]
    public string Title { get => _title;
    set
        {
            _title = value;
            // Automatically sync the slug whenever the name changes
            Slug = StringSanitizer.Normalize(_title);
        }
     } 

    /// <summary>
    /// Gets or sets a unique, URL-friendly slug for the entity.
    /// This is used for SEO-friendly routing and identifier resolution.
    /// </summary>
    [Required, MaxLength(128), Column("slug")]
    public string Slug { get; set; } = "";

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was first created.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets the UTC timestamp when the entity was last modified.
    /// </summary>
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets a brief description of the entity.
    /// </summary>
    [MaxLength(4096)]
    public string? ShortDesc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is accessible to the public.
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Gets or sets a collection of unique identifiers for tags associated with this entity.
    /// This is ignored during JSON serialization to prevent circular references or deep nesting.
    /// </summary>
    [JsonIgnore]
    public List<Guid> TagIds { get; init; } = new ();

    /// <summary>
    /// Gets or sets a hint for the entity type, used during polymorphic deserialization or mapping.
    /// </summary>
    public virtual string? TypeHint {get;set;}
}

