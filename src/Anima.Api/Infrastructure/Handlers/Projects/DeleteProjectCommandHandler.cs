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

public class DeleteProjectCommandHandler : BaseCommandHandler<DeleteProjectCommand, SuccessResponseDto>
{

    public DeleteProjectCommandHandler(ILogger<DeleteProjectCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core)
    {
        
    }

    protected override PermissionsEnum RequiredPermission => PermissionsEnum.CanDelete;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Admin;

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeAuthenticated;

    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.Entity;

    protected override async Task<ApiResponseDto<SuccessResponseDto>> ExecuteAsync(DeleteProjectCommand command,RequestContext context)
    {
        var project = await _db.Projects.FindAsync(context.ProjectId);
        if (project == null) throw new Exception("Project not found");

        _db.Projects.Remove(project);
        await _db.SaveChangesAsync();

        return ApiResponseDto<SuccessResponseDto>.Success(new SuccessResponseDto() { Success = true });
    }
}
