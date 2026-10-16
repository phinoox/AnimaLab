namespace Anima.Api.Infrastructure.Handlers.Projects;

using Anima.Api.CoreServices.Interfaces;
using Anima.Api.Infrastructure.Commands;
using Anima.Core.Base.Commands;
using Anima.Core.Dtos.Shared;
using Anima.Core.Models.Authentication;
using Anima.Core.Models.Projects;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class CreateProjectCommandHandler : WriteCommandHandler<CreateProjectCommand, MetaIdentityResponseDto>
{
    
    public CreateProjectCommandHandler(ILogger<CreateProjectCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core)
    {
        
    }

    protected override PermissionsEnum RequiredPermission => PermissionsEnum.CanCreate;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Admin;

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;

    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.None;

    protected override async Task<ApiResponseDto<MetaIdentityResponseDto>> ExecuteAsync(CreateProjectCommand command,RequestContext context)
    {
        var meta = new ProjectMetaInfo{
            Id = Guid.NewGuid(),
            CreatedAt = DateTime.Now,
            LastModifiedAt = DateTime.Now,
            CreatedByUserId = _userId
            };
        await MapProperties<ProjectMetaInfo>(command.Meta,meta);

        var project = new Project
        {
            Id = meta.Id,
            
        };

        _db.ProjectMetaInfos.Add(meta);
        _db.Projects.Add(project);

        await _db.SaveChangesAsync();

        return  ApiResponseDto<MetaIdentityResponseDto>.Success(new MetaIdentityResponseDto()
        { 
            Id = meta.Id, 
            Slug = meta.Slug 
        });
    }
}
