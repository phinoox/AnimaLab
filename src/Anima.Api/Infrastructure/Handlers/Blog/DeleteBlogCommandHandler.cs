namespace Anima.Api.Infrastructure.Handlers.Blog;

using Anima.Api.CoreServices.Interfaces;
using Anima.Api.Infrastructure.Commands;
using Anima.Core.Base.Commands;
using Anima.Core.Models.Authentication;
using Anima.Core.Models.Blog;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class DeleteBlogCommandHandler : WriteCommandHandler<DeleteBlogCommand, SuccessResponseDto>
{
    

    public DeleteBlogCommandHandler(ILogger<DeleteBlogCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core)
    {
    }

    protected override PermissionsEnum RequiredPermission => PermissionsEnum.CanDelete;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Admin;

   protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;

    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.Entity;

    protected override async Task<ApiResponseDto<SuccessResponseDto>> ExecuteAsync(DeleteBlogCommand command,RequestContext context)
    {
        var post = await _db.BlogPosts.FindAsync(context.EntityId);
        if (post == null) return ApiResponseDto<SuccessResponseDto>.NotFound("Blog post not found");

        _db.BlogPosts.Remove(post);
        await _db.SaveChangesAsync();

        return ApiResponseDto<SuccessResponseDto>.Success(new() { Success = true });
    }
}
