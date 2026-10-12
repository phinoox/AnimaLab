using Anima.Api.CoreServices.Interfaces;
using Anima.Api.Infrastructure.Commands;
using Anima.Core.Base.Commands;
using Anima.Core.Models.Authentication;
using Anima.Core.Models.Projects;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;

namespace Anima.Api.Infrastructure.Handlers.Projects;

public class GetProjectListCommandHandler : BaseCommandHandler<GetProjectListCommand, IEnumerable<ProjectMetaInfo>>
{
    public GetProjectListCommandHandler(ILogger<GetProjectListCommand> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core) { }

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;
    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.None; // Or Project if you want to validate scope even on Get
    protected override PermissionsEnum RequiredPermission => PermissionsEnum.CanView;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Default;

    protected override async Task<ApiResponseDto<IEnumerable<ProjectMetaInfo>>> ExecuteAsync(GetProjectListCommand command,RequestContext context)
    {
        var projects = await _db.ProjectMetaInfos.Where(pmi => pmi.CreatedByUserId == _userId).ToListAsync();
        if (projects == null) return ApiResponseDto<IEnumerable<ProjectMetaInfo>>.NotFound("no Projects not found.");

        return ApiResponseDto<IEnumerable<ProjectMetaInfo>>.Success(projects);
    }
}