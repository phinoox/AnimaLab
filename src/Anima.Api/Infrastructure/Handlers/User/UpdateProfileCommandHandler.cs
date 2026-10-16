namespace Anima.Api.Infrastructure.Handlers.User;

using Anima.Api.CoreServices.Interfaces;
using Anima.Api.Infrastructure.Commands;
using Anima.Core.Base.Commands;
using Anima.Core.Models.Authentication;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class UpdateProfileCommandHandler : WriteCommandHandler<UpdateProfileCommand, SuccessResponseDto>
{

    public UpdateProfileCommandHandler(ILogger<UpdateProfileCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core)
    {
    }

    protected override PermissionsEnum RequiredPermission => PermissionsEnum.Default;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Default;

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;

    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.Entity;

    protected override async Task<ApiResponseDto<SuccessResponseDto>> ExecuteAsync(UpdateProfileCommand command,RequestContext context)
    {
        if (_userId == Guid.Empty)
            return ApiResponseDto<SuccessResponseDto>.Unauthorized("User not authenticated.");

        var meta = await _db.UserMetaInfos.FindAsync(_userId);

        if(meta == null)
            return ApiResponseDto<SuccessResponseDto>.NotFound("Profile not found");    
        await MapProperties<UserMetaInfo>(command.Data,meta);
        _db.UserMetaInfos.Update(meta);
        await _db.SaveChangesAsync();         

        return ApiResponseDto<SuccessResponseDto>.Success(new() { Success = true });
    }
}
