namespace Anima.Core.Models.ContentBase;

/// <summary>
/// Represents the status of a review process for content or tasks.
/// </summary>
public enum ReviewStatusEnum
{
    /// <summary>The review is currently pending and has not yet been processed.</summary>
    Pending,

    /// <summary>The content or task has been reviewed and approved.</summary>
    Approved,

    /// <summary>The content or task has been reviewed and rejected.</summary>
    Rejected
}