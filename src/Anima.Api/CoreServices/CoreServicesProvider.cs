using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Models.Authentication;

namespace Anima.Api.CoreServices;

/// <summary>
/// Provides a centralized implementation of <see cref="ICoreServicesProvider"/>, 
/// acting as the primary service locator for core cross-cutting concerns within the API.
/// </summary>
public class CoreServicesProvider : ICoreServicesProvider
{
    private readonly IAuditService _auditService;
    private readonly IPermissionEngine _permissionEngine;
    private readonly IEntityMapper _entityMapper;
    private readonly ISlugResolverService _slugResolverService;
    private IUserContext? _userContext;

    /// <summary>
    /// Initializes a new instance of the <see cref="CoreServicesProvider"/> class.
    /// </summary>
    /// <param name="auditService">The service for recording audit logs.</param>
    /// <param name="permissionEngine">The engine for evaluating user permissions.</param>
    /// <param name="entityMapper">The mapper for DTO-to-Entity operations.</param>
    /// <param name="slugResolverService">The service for resolving URL slugs.</param>
    /// <param name="userContext">The initial user context.</param>
    public CoreServicesProvider(IAuditService auditService,
                                IPermissionEngine permissionEngine,
                                IEntityMapper entityMapper,
                                ISlugResolverService slugResolverService,
                                IUserContext? userContext)
    {
        _auditService = auditService;
        _permissionEngine = permissionEngine;
        _entityMapper = entityMapper;
        _slugResolverService = slugResolverService;
        _userContext = userContext;
    }

    /// <summary>
    /// Gets the audit service.
    /// </summary>
    public IAuditService AuditService { get => _auditService; }

    /// <summary>
    /// Gets the permission engine.
    /// </summary>
    public IPermissionEngine PermissionEngine { get => _permissionEngine; }

    /// <summary>
    /// Gets or sets the current user context.
    /// </summary>
    public IUserContext? UserContext { get => _userContext; set => _userContext = value; }

    /// <summary>
    /// Gets the slug resolver service.
    /// </summary>
    public ISlugResolverService SlugResolverService { get => _slugResolverService; }

    /// <summary>
    /// Gets the entity mapper.
    /// </summary>
    public IEntityMapper EntityMapper { get => _entityMapper; }
}