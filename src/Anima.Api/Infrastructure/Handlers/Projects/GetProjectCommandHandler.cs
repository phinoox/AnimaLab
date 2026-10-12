using Anima.Api.CoreServices.Interfaces;
using Anima.Api.Infrastructure.Commands;
using Anima.Core.Base.Commands;
using Anima.Core.Models.Authentication;
using Anima.Core.Models.Projects;
using Anima.Data.Contexts;

namespace Anima.Api.Infrastructure.Handlers.Projects;

public class GetProjectCommandHandler : BaseCommandHandler<GetProjectCommand, ProjectMetaInfo>
{
    public GetProjectCommandHandler(ILogger<GetProjectCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core) { }

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;
    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.Project; // Or Project if you want to validate scope even on Get
    protected override PermissionsEnum RequiredPermission => PermissionsEnum.CanView;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Default;

    protected override async Task<ApiResponseDto<ProjectMetaInfo>> ExecuteAsync(GetProjectCommand command,RequestContext context)
    {
        // Note: The ID is NOT in the command. We must assume the BaseCommandHandler 
        // provides a way to access the context, or we pass it via the HandleAsync method.
        // For this example, I'll assume the base class makes the current RequestContext accessible.
        
        var projectId = context.ProjectId; // Accessing context from the base class
        if (projectId == null) return ApiResponseDto<ProjectMetaInfo>.BadRequest("Project ID missing in route");

        var meta = await _db.ProjectMetaInfos.FindAsync(projectId.Value);
        if (meta == null) return ApiResponseDto<ProjectMetaInfo>.NotFound("Project not found.");

        return ApiResponseDto<ProjectMetaInfo>.Success(meta);
    }
}