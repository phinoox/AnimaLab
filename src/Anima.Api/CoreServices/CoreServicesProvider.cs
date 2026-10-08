using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Models.Authentication;

namespace Anima.Api.CoreServices;



public class CoreServicesProvider : ICoreServicesProvider
{
    private IAuditService _auditService;
    private IPermissionEngine _permissionEngine;

    public IEntityMapper _entityMapper; 
    
    private IUserContext? _userContext;

    private ISlugResolverService _slugResolverService;

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

    public IAuditService AuditService { get => _auditService; }
    public IPermissionEngine PermissionEngine { get => _permissionEngine; }
    public IUserContext? UserContext { get => _userContext; set => _userContext = value; }

    public ISlugResolverService SlugResolverService {get => _slugResolverService;}

    public IEntityMapper EntityMapper { get => _entityMapper; }
}