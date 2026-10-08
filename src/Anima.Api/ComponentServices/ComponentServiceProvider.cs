namespace Anima.Api.ComponentServices;

using Anima.Api.ComponentServices.Interfaces;
using Anima.Core.Interfaces.Component;

public class ComponentServiceProvider : IComponentServiceProvider
{
    public ICommentService Comments { get; }
    public ITagService Tags { get; }
    public IReviewStatusService ReviewStatus { get; }

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