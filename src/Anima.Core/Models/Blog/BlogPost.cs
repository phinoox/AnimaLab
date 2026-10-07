using Anima.Core.Models.ContentBase;

namespace Anima.Core.Models.Blog;

/// <summary>
/// Specific details for a Blog Page content item.
/// </summary>
public class BlogPost
{
    [Key]
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// The unique identifier of the associated ContentMetaInfo.
    /// </summary>
    [Required]
    public Guid MetaInfoId { get; set; }

    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo MetaInfo { get; set; } = null!;

    /// <summary>
    /// The date and time when the blog post was officially published.
    /// </summary>
    public DateTime? PublishedAt { get; set; }

    /// <summary>
    /// The unique identifier of the user who authored this blog post.
    /// </summary>
    [Required]
    public Guid AuthorId { get; set; }

    /// <summary>
    /// A short summary or teaser used for preview cards and search results.
    /// </summary>
    [MaxLength(10000)]
    public string? BlogText { get; set; }
}
