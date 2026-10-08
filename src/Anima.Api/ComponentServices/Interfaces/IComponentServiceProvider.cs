namespace Anima.Api.ComponentServices.Interfaces;

using Anima.Core.Models.Authentication;

/// <summary>
/// Defines a contract for providing access to specialized component services within the API.
/// These services act as "sub-resource workers" that are composed by Domain Services 
/// to handle specific domain logic.
/// </summary>
public interface IComponentServiceProvider
{
    /// <summary>
    /// Gets the service responsible for managing comments.
    /// </summary>
    ICommentService Comments { get; }

    /// <summary>
    /// Gets the service responsible for managing tags.
    /// </summary>
    ITagService Tags { get; }

    /// <summary>
    /// Gets the service responsible for managing review statuses.
    /// </summary>
    IReviewStatusService ReviewStatus { get; }
}