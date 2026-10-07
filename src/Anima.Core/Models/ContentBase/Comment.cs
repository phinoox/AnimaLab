namespace Anima.Core.Models.ContentBase;

using System.ComponentModel.DataAnnotations;


 /// <summary>
 /// Represents a user-provided comment attached to a specific content item or entity.
 /// </summary>
 [ModelDependency(typeof(ContentMetaInfo))]
public class Comment : EntityBase
{
    /// <summary>
    /// The ID of the target entity (e.g., a Scene or Character) this comment belongs to.
    /// This acts as the anchor link for $1:N$ relationships.
    /// </summary>
    [Required]
    public Guid TargetId { get; set; }
    
    /// <summary>
    /// Gets or sets the identifier of the user who authored this comment.
    /// </summary>
    [Required] 
    public Guid AuthorUserId { get; init; }
    
    /// <summary>
    /// The text content of the comment.
    /// </summary>
    [MaxLength(4096)] 
    public string Text { get; set; } = "";
    
    /// <summary>
    /// The ID of the parent comment, if this is a reply in a threaded conversation.
    /// </summary>
    public Guid? ParentCommentId { get; init; }
    
    /// <summary>
    /// Gets or sets the timestamp when the comment was created.
    /// </summary>
    public DateTime CreatedAt { get; init; } = DateTime.UtcNow;
}
