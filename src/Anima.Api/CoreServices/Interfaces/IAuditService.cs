namespace Anima.Api.CoreServices.Interfaces;

/// <summary>
/// Service for recording business-level audit events and activity logs.
/// </summary>
public interface IAuditService
{
    /// <summary>
    /// Records a business-level audit event into the database.
    /// </summary>
    /// <param name="projectId">The project scope this event belongs to.</param>
    /// <param name="action">The action performed (e.g., "Created", "Updated").</param>
    /// <param name="relatedEntityType">The type of the entity being acted upon.</param>
    /// <param name="relatedEntityId">The unique identifier of the related entity.</param>
    /// <param name="description">A human-readable description of the event.</param>
    Task LogDbAsync(Guid? projectId, string action, string relatedEntityType, Guid? relatedEntityId = null, string? description = null);
}