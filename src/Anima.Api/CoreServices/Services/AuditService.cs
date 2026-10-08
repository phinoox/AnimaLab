using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Models.Authentication;
using Anima.Data.Contexts;

namespace Anima.Api.CoreServices.Services;

public class AuditService : IAuditService
{
    private readonly AppDbContext _db;
    private readonly IUserContext _userContext;
    private readonly ILogger<AuditService> _logger;

    public AuditService(CoreDbContext db, IUserContext userContext, ILogger<AuditService> logger)
    {
        _db = db;
        _userContext = userContext;
        _logger = logger;
    }

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