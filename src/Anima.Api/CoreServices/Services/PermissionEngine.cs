using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Models.Authentication;
using Anima.Core.Models.ContentBase;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.CoreServices.Services;

/// <summary>
/// The central authority for evaluating authorization requests across the system.
/// This implementation handles project-level membership and visibility rules, including handling deleted projects.
/// </summary>
public class PermissionEngine : IPermissionEngine
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Initializes a new instance of the <see cref="PermissionEngine"/> class.
    /// </summary>
    /// <param name="db">The application database context.</param>
    public PermissionEngine(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Evaluates if a user has sufficient permission to perform an action within a specific project.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="projectId">The ID of the project being accessed.</param>
    /// <param name="minRole">The minimum required role to perform the action.</param>
    /// <returns>An <see cref="AccessResult"/> indicating whether access is granted, denied, or if the resource was not found.</returns>
    public async Task<AccessResult> CheckAccessAsync(Guid userId, Guid projectId, ProjectMemberRoleEnum minRole)
    {
        if (userId == Guid.Empty)
            return new AccessResult(AccessResultStatus.Unauthorized, "You need to be logged in to access this Project");

        // 1. Get the project regardless of deletion status using IgnoreQueryFilters to ensure we can check visibility rules for deleted projects
        var project = await _db.Projects
            .IgnoreQueryFilters()
            .SingleOrDefaultAsync(p => p.Id == projectId);

        if (project == null)
            return new AccessResult(AccessResultStatus.NotFound, "Project not found.");

        // 2. Determine User's Relationship/Role within the project
        bool isOwner = project.UserId == userId;
        var member = await _db.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);

        // 3. Apply Visibility Rule (The "Who can see a deleted project" rule)
        if (project.IsDeleted)
        {
            // Only Owners or Admins are allowed to 'see' a deleted project
            bool isAdmin = member != null && member.Role == ProjectMemberRoleEnum.Admin;
            if (!isOwner && !isAdmin)
            {
                return new AccessResult(AccessResultStatus.NotFound, "Project not found.");
            }
        }

        // 4. Apply PermissionsEnum Rule (The "Can they perform this action" rule)
        // Owners always have access to their own projects
        if (isOwner) return new AccessResult(AccessResultStatus.Allowed, ""); 

        // Check if the user is a member and has the required role level
        if (member == null || member.Role < minRole)
            return new AccessResult(AccessResultStatus.Forbidden, "You do not have sufficient access to this project");

         return new AccessResult(AccessResultStatus.Allowed, "");
    }
    
    /// <summary>
    /// Determines if a given entity (specifically ContentMetaInfo in this implementation) belongs to the specified project.
    /// </summary>
    /// <param name="projectId">The ID of the project.</param>
    /// <param name="targetId">The unique identifier of the content meta information.</param>
    /// <returns>True if the entity exists within the project scope; otherwise, false.</returns>
    public async Task<bool> IsTargetInProjectAsync(Guid projectId, Guid targetId)
    {
         var exists = await _db.Set<ContentMetaInfo>()
            .AnyAsync(m => m.Id == targetId && m.ProjectId == projectId);

        return exists;
    }

    /// <summary>
    /// Retrieves the specific role of a user within a given project.
    /// </summary>
    /// <param name="userId">The unique identifier of the user.</param>
    /// <param name="projectId">The ID of the project.</param>
    /// <returns>The <see cref="ProjectMemberRoleEnum"/> assigned to the user in that project.</returns>
    /// <exception cref="NullReferenceException">Thrown if no membership record is found for the user in the project.</exception>
    public async Task<ProjectMemberRoleEnum> GetUserRole(Guid userId, Guid projectId)
    {
        var member = await _db.ProjectMembers
            .FirstOrDefaultAsync(pm => pm.ProjectId == projectId && pm.UserId == userId);
            
        if (member == null)
            throw new NullReferenceException($"No membership record found for user {userId} in project {projectId}");

        return member.Role;
    }
}