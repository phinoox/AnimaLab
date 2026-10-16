using Anima.Api.CoreServices.Interfaces;
using Anima.Api.Infrastructure.Commands;
using Anima.Core.Base.Commands;
using Anima.Core.Models.Authentication;
using Anima.Core.Models.ContentBase;
using Anima.Data.Contexts;

namespace Anima.Api.Infrastructure.Handlers.Blog;

public class GetBlogCommandHandler : ReadCommandHandler<GetBlogCommand, ContentMetaInfo>
{
    public GetBlogCommandHandler(ILogger<GetBlogCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core) { }

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;
    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.Entity;
    protected override PermissionsEnum RequiredPermission => PermissionsEnum.CanView;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Default;

    protected override async Task<ApiResponseDto<ContentMetaInfo>> ExecuteAsync(GetBlogCommand command,RequestContext context)
    {
        var projectId = context.ProjectId;
        var blogId = context.EntityId;

        if (projectId == null || blogId == null) 
            return ApiResponseDto<ContentMetaInfo>.BadRequest("Scope IDs missing in route");

        var meta = await _db.ContentMetaInfos.FindAsync(blogId.Value);
        // ... validation logic (check if blog belongs to project) ...

        if (meta == null) return ApiResponseDto<ContentMetaInfo>.NotFound("Blog post not found.");

        return ApiResponseDto<ContentMetaInfo>.Success(meta);
    }
}