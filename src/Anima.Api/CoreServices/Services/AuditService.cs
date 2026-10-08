using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Models.Authentication;
using Anima.Data.Contexts;

namespace Anima.Api.CoreServices.Services;

/// <summary>
/// Provides functionality for logging database-level activities and audit trails.
/// </summary>
public class AuditService : IAuditService
{
    private readonly AppDbContext _db;
    private readonly IUserContext _userContext;
    private readonly ILogger<AuditService> _logger;

    public AuditService(AppDbContext db, IUserContext userContext, ILogger<AuditService> logger)
    {
        _db = db;
        _userContext = userContext;
        _logger = logger;
    }

    /// <summary>
    /// Logs a database activity to the activity log.
    /// </summary>
    /// <param name="projectId">The ID of the project this activity belongs to.</param>
    /// <param name="action">A description of the action performed (e.g., "Created", "Updated").</param>
    /// <param name="relatedEntityType">The type of the entity that was affected.</param>
    /// <param name="relatedEntityId">The ID of the related entity, if applicable.</param>
    /// <param name="description">Optional additional details about the activity.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    /// <exception cref="ArgumentException">Thrown when projectId or user is not available.</exception>
    public async Task LogDbAsync(Guid? projectId, string action, string relatedEntityType, Guid? relatedEntityId = null, string? description = null)
    {
        var actualProjectId = projectId ?? throw new ArgumentException("ProjectId is required for activity logging.");
        var actualUser = _userContext.CurrentUser?.Id ?? throw new ArgumentException("User is required for activity logging.");

        var log = new ActivityLog
        {
            ProjectId = actualProjectId,
            UserId = actualUser,
            Action = action,
            RelatedEntityType = relatedEntityType,
            RelatedEntityId = relatedEntityId,
            Description = description,
            CreatedAt = DateTime.UtcNow
        };

        _db.ActivityLogs.Add(log);
        await _db.SaveChangesAsync();
    }
}