namespace Anima.Api.Infrastructure.Handlers.Authentication;

using Anima.Api.CoreServices.Interfaces;
using Anima.Api.Infrastructure.Commands;
using Anima.Api.Utils;
using Anima.Core.Base.Commands;
using Anima.Core.Dtos.Authentication;
using Anima.Core.Models.Authentication;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class RegisterCommandHandler : WriteCommandHandler<RegisterCommand, MetaIdentityResponseDto>
{

    public RegisterCommandHandler(ILogger<RegisterCommandHandler> logger, AppDbContext db, ICoreServicesProvider core) 
        : base(logger, db, core)
    {
    }

    protected override PermissionsEnum RequiredPermission => PermissionsEnum.Default; // Registration is public/system level
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Default;

    protected override IdentityRequirementEnum IdentityRequirement =>  IdentityRequirementEnum.MustBeUnauthenticated;

    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.None;

    protected override async Task<ApiResponseDto<MetaIdentityResponseDto>> ExecuteAsync(RegisterCommand command,RequestContext context)
    {
        var registerDto = command.Data;

        var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Email == registerDto.Email);
        if (existingUser != null)
            return ApiResponseDto<MetaIdentityResponseDto>.Conflict($"A user with email '{registerDto.Email}' already exists.");

        var userId = Guid.NewGuid();

        var user = new User
        {
            Id = userId,
            UserName = registerDto.UserName ?? registerDto.Email,
            Email = registerDto.Email,
            PasswordHash = PasswordHasher.Hash(registerDto.Password),
            EmailConfirmed = true,
            
        };

        var metaInfo = new UserMetaInfo
        {
            Id = userId,
            CreatedByUserId = UserConstants.SystemUserId,
            Title = user.UserName,
            IsPublic = false,
            CreatedAt = DateTime.UtcNow,
            LastModifiedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        _db.UserMetaInfos.Add(metaInfo);

        await _db.SaveChangesAsync();

        return ApiResponseDto<MetaIdentityResponseDto>.Success(new MetaIdentityResponseDto
        {
            Id = userId,
            Slug = metaInfo.Slug // Assuming Slug is generated/set in MetaInfo logic or via constructor
        });
    }
}
