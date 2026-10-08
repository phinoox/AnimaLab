namespace Anima.Api.CoreServices.Interfaces;

/// <summary>
/// Defines a contract for services that record business-level audit events and activity logs.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Records a business-level audit event into the database asynchronously.
    /// </summary>
    /// <param name="projectId">The project scope this event belongs to.</param>
    /// <param name="action">A description of the action performed (e.g., "Created", "Updated").</param>
    /// <param name="relatedEntityType">The type of the entity being acted upon (e.g., "Project", "Content").</param>
    /// <param name="relatedEntityId">The unique identifier of the related entity, if applicable.</param>
    /// <param name="description">A human-readable description providing additional context for the event.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task LogDbAsync(Guid? projectId, string action, string relatedEntityType, Guid? relatedEntityId = null, string? description = null);
}