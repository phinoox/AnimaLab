using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Models.Authentication;

namespace Anima.Api.CoreServices.Interfaces;

/// <summary>
/// Provides a centralized access point for all core services used across the API.
/// This acts as a service locator for cross-cutting concerns like auditing, permissions, and mapping.
/// </summary>
public interface ICoreServicesProvider
{
    /// <summary>
    /// Gets the audit service for logging activities.
    /// </summary>
    IAuditService AuditService { get; }

    /// <summary>
    /// Gets the permission engine for evaluating user permissions.
    /// </summary>
    IPermissionEngine PermissionEngine { get; }

    /// <summary>
    /// Gets the slug resolver service for generating and resolving URL-friendly slugs.
    /// </summary>
    ISlugResolverService SlugResolverService { get; }

    /// <summary>
    /// Gets the entity mapper for DTO-to-Entity mapping.
    /// </summary>
    IEntityMapper EntityMapper { get; }

    /// <summary>
    /// Gets or sets the current user context, providing information about the authenticated user.
    /// </summary>
    IUserContext? UserContext { get; set; }
}