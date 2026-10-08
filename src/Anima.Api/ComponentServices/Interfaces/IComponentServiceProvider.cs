namespace Anima.Api.ComponentServices.Interfaces;


using Anima.Core.Models.Authentication;

public interface IComponentServiceProvider
{
    // These are the sub-resource workers (the "Muscle") 
    // accessed through the provider for composition within DomainServices.

    ICommentService Comments { get; }
    ITagService Tags { get; }
    IReviewStatusService ReviewStatus { get; }
}
