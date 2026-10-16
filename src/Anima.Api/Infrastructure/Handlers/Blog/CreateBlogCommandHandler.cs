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

public class CreateBlogCommandHandler : WriteCommandHandler<CreateBlogCommand, MetaIdentityResponseDto>
{

    public CreateBlogCommandHandler(ILogger<CreateBlogCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core)
    {
    }

    protected override PermissionsEnum RequiredPermission => PermissionsEnum.CanCreate;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Admin;

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;

    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.Project;

    protected override async Task<ApiResponseDto<MetaIdentityResponseDto>> ExecuteAsync(CreateBlogCommand command,RequestContext context)
    {
        var newId = Guid.NewGuid();

        var meta = new ContentMetaInfo
        {
            Id = newId,
            CreatedByUserId = _userId,
            ParentId = context.ProjectId!.Value,
            TypeHint = typeof(BlogPost).Name,
            ReviewStatusId = null
        };

        await MapProperties<ContentMetaInfo>(command.Meta,meta);

        var blogPost = new BlogPost
        {
            Id = newId,
            AuthorId = _userId,
        };

        _db.ContentMetaInfos.Add(meta);
        _db.BlogPosts.Add(blogPost);
        await _db.SaveChangesAsync();
        

        return ApiResponseDto<MetaIdentityResponseDto>.Success(new() 
        { 
            Id = meta.Id, 
            Slug = meta.Slug 
        });
    }
}
