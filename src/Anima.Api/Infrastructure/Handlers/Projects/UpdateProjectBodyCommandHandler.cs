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

public class UpdateProjectBodyCommandHandler : WriteCommandHandler<UpdateProjectBodyCommand, SuccessResponseDto>
{
    

    public UpdateProjectBodyCommandHandler(ILogger<UpdateProjectBodyCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core)
    {
    
    }

    protected override PermissionsEnum RequiredPermission => PermissionsEnum.CanEdit;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Editor;

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;

    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.Project;

    protected override async Task<ApiResponseDto<SuccessResponseDto>> ExecuteAsync(UpdateProjectBodyCommand command,RequestContext context)
    {
        var project = await _db.Projects.FindAsync(context.ProjectId);
        if (project == null) throw new Exception("Project not found");

        // Map properties from DTO to Entity
        await MapProperties<Project>(command.Data,project);
        
        _db.Projects.Update(project);
        await _db.SaveChangesAsync();

        return  ApiResponseDto<SuccessResponseDto>.Success(new SuccessResponseDto() { Success = true });
    }
}
