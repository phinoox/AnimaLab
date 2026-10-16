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

public class UpdateProjectMetaCommandHandler : WriteCommandHandler<UpdateProjectMetaCommand, SuccessResponseDto>
{
    

    public UpdateProjectMetaCommandHandler(ILogger<UpdateProjectMetaCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core)
    {
    
    }

    protected override PermissionsEnum RequiredPermission => PermissionsEnum.CanEdit;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Editor;

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;

    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.Project;

    protected override async Task<ApiResponseDto<SuccessResponseDto>> ExecuteAsync(UpdateProjectMetaCommand command,RequestContext context)
    {
        var meta = await _db.ProjectMetaInfos.FindAsync(context.ProjectId);
        if (meta == null) throw new Exception("Project Meta not found");

        await MapProperties<ProjectMetaInfo>(command.Meta,meta);
        
        _db.ProjectMetaInfos.Update(meta);
        await _db.SaveChangesAsync();

        return ApiResponseDto<SuccessResponseDto>.Success(new SuccessResponseDto() { Success = true });
    }
}
