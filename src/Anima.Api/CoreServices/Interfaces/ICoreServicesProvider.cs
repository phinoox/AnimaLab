using Anima.Api.CoreServices.Interfaces;
using Anima.Core.Models.Authentication;

public interface ICoreServicesProvider
{
    IAuditService AuditService { get; }
    IPermissionEngine PermissionEngine { get; }

    ISlugResolverService SlugResolverService {get;}

    IEntityMapper EntityMapper {get;}
    IUserContext? UserContext { get; set; }
}