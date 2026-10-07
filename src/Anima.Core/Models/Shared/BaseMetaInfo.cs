// =============================================================================
// Anima.Core - Shared Domain Models & Interfaces
// =============================================================================

using System.Text.Json.Serialization;

namespace Anima.Core.Models.Shared;

/// <summary>
/// The universal identity anchor for all entities in the Anima universe.
/// Provides the core "DNA" (Identity) that remains consistent across any scope and serves as a base for specialized meta-information models.
/// </summary>
public abstract class BaseMetaInfo
{
    private string _title = "";

    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Key]
    public Guid Id { get; init; } = Guid.NewGuid();

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
    /// </summary>
    [Required, MaxLength(128), Column("slug")]
    public string Slug { get; set; } = "";

    /// <summary>
    /// Gets or sets the timestamp when the entity was first created.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

    /// <summary>
    /// Last modification timestamp.
    /// </summary>
    public DateTime LastModifiedAt { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Gets or sets a brief description of the entity.
    /// </summary>
    [MaxLength(4096)]
    public string? ShortDesc { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether the entity is publicly visible.
    /// </summary>
    public bool IsPublic { get; set; } = false;

    /// <summary>
    /// Gets or sets a collection of unique identifiers for tags associated with this entity.
    /// </summary>
    [JsonIgnore]
    public List<Guid> TagIds { get; init; } = new ();

    public virtual string? TypeHint {get;set;}
}

