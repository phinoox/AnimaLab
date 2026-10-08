namespace Anima.Api.ComponentServices;

using Anima.Api.ComponentServices.Interfaces;


/// <summary>
/// Provides an implementation of <see cref="IComponentServiceProvider"/>, 
/// acting as a container for specialized component services used by Domain Services.
/// </summary>
public class ComponentServiceProvider : IComponentServiceProvider
{
    /// <summary>
    /// Gets the service responsible for managing comments.
    /// </summary>
    public ICommentService Comments { get; }

    /// <summary>
    /// Gets the service responsible for managing tags.
    /// </summary>
    public ITagService Tags { get; }

    /// <summary>
    /// Gets the service responsible for managing review statuses.
    /// </summary>
    public IReviewStatusService ReviewStatus { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ComponentServiceProvider"/> class.
    /// </summary>
    /// <param name="commentService">The implementation of the comment service.</param>
    /// <param name="tagService">The implementation of the tag service.</param>
    /// <param name="reviewStatusService">The implementation of the review status service.</param>
    public ComponentServiceProvider(
        ICommentService commentService,
        ITagService tagService,
        IReviewStatusService reviewStatusService)
    {
        Comments = commentService;
        Tags = tagService;
        ReviewStatus = reviewStatusService;
    }
}