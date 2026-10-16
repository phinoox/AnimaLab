namespace Anima.Api.Infrastructure.Handlers.Blog;

using Anima.Api.CoreServices.Interfaces;
using Anima.Api.Infrastructure.Commands;
using Anima.Core.Base.Commands;
using Anima.Core.Models.Authentication;
using Anima.Core.Models.Blog;
using Anima.Core.Models.ContentBase;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class UpdateBlogMetaCommandHandler : WriteCommandHandler<UpdateBlogMetaCommand, SuccessResponseDto>
{

    public UpdateBlogMetaCommandHandler(ILogger<UpdateBlogMetaCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core)
    {
    }

    protected override PermissionsEnum RequiredPermission => PermissionsEnum.CanEdit;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Editor;

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;

    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.Entity;

    protected override async Task<ApiResponseDto<SuccessResponseDto>> ExecuteAsync(UpdateBlogMetaCommand command,RequestContext context)
    {
        var meta = await _db.ContentMetaInfos.FindAsync(context.EntityId);
        if (meta == null) return ApiResponseDto<SuccessResponseDto>.NotFound("Blog Meta not found");

        await MapProperties<ContentMetaInfo>(command.Meta,meta);

        _db.ContentMetaInfos.Update(meta);
        await _db.SaveChangesAsync();

        return ApiResponseDto<SuccessResponseDto>.Success(new() { Success = true });
    }
}
