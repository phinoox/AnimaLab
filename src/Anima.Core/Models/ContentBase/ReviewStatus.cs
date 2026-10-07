namespace Anima.Core.Models.ContentBase;

using System.ComponentModel.DataAnnotations;
using Anima.Core.Models.Authentication;
using Anima.Core.Utils;

/// <summary>
/// Represents a review status for a specific piece of content.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class ReviewStatus : EntityBase
{
    /// <summary>
    /// The current status of the review process.
    /// </summary>
    public ReviewStatusEnum Status { get; set; }
    
    /// <summary>
    /// The ID of the user who performed the review.
    /// </summary>
    public Guid? ReviewedByUserId { get; set; }

    /// <summary>
    /// Navigation property for the reviewer.
    /// </summary>
    [ForeignKey("ReviewedByUserId")]
    public virtual User? Reviewer { get; set; }
    
    /// <summary>
    /// The qualitative feedback or notes provided by the reviewer.
    /// </summary>
    [MaxLength(4096)]
    public string? ReviewComment { get; set; }
    
    /// <summary>
    /// The timestamp when the review was completed.
    /// </summary>
    public DateTime? ReviewedAt { get; set; }

    /// <summary>
    /// The timestamp when the review record was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }
}
