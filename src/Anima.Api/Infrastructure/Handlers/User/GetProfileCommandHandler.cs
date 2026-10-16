namespace Anima.Api.Infrastructure.Handlers.User;

using Anima.Api.CoreServices.Interfaces;
using Anima.Api.Infrastructure.Commands;
using Anima.Core.Base.Commands;
using Anima.Core.Models.Authentication;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class GetProfileCommandHandler : ReadCommandHandler<GetProfileCommand, UserMetaInfo>
{

    public GetProfileCommandHandler(ILogger<GetProfileCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core)
    {
    }

    protected override PermissionsEnum RequiredPermission => PermissionsEnum.Default;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Default;

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;

    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.None;

    protected override async Task<ApiResponseDto<UserMetaInfo>> ExecuteAsync(GetProfileCommand command,RequestContext context)
    {
        if (_userId == Guid.Empty) 
            return ApiResponseDto<UserMetaInfo>.Unauthorized("User not authenticated.");

        var user = await _db.UserMetaInfos.FirstOrDefaultAsync(u => u.Id == _userId);
        if (user == null)
            return ApiResponseDto<UserMetaInfo>.NotFound("User not found.");

        return ApiResponseDto<UserMetaInfo>.Success(user);
    }
}
