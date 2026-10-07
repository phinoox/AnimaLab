namespace Anima.Core.Models.Tagging;

/// <summary>
/// Represents a universal tag used for categorization and discovery across the entire system.
/// Tags can be applied to various content types via relationship entities.
/// </summary>
[ModelDependency(typeof(RootMarker))]
public class MetaTag
{
    /// <summary>
    /// Unique identifier for this tag.
    /// </summary>
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    private string _name = "";

    /// <summary>
    /// The human-readable name of the tag (e.g., "Fantasy", "High Stakes").
    /// Changing the name automatically updates the slug.
    /// </summary>
    [Required, MaxLength(128)]
    public string Name
    {
        get => _name;
        set
        {
            _name = value;
            // Automatically sync the slug whenever the name changes
            Slug = StringSanitizer.Normalize(_name);
        }
    }

    /// <summary>
    /// A unique, URL-friendly slug derived from the tag's name.
    /// </summary>
        [Required, MaxLength(128), Column("slug")]
        public string Slug { get; set; } = "";
}