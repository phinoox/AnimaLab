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

public class SignInCommandHandler : WriteCommandHandler<SignInCommand, AuthResponseDto>
{
    private readonly JwtTokenService _jwtService;

    public SignInCommandHandler(ILogger<SignInCommandHandler> logger, AppDbContext db, ICoreServicesProvider core, JwtTokenService jwtService) 
        : base(logger, db, core)
    {
        _jwtService = jwtService;
    }

    protected override PermissionsEnum RequiredPermission => PermissionsEnum.Default;
    protected override ProjectMemberRoleEnum MinRole => ProjectMemberRoleEnum.Default;

    protected override IdentityRequirementEnum IdentityRequirement => IdentityRequirementEnum.MustBeUnauthenticated;

    protected override ScopeRequirementEnum ScopeRequirement => ScopeRequirementEnum.None;

    protected override async Task<ApiResponseDto<AuthResponseDto>> ExecuteAsync(SignInCommand command,RequestContext context)
    {
        var signInDto = command.Data;
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == signInDto.Email);

        if (user == null || !PasswordHasher.Verify(signInDto.Password, user.PasswordHash))
            return ApiResponseDto<AuthResponseDto>.Unauthorized("Invalid email or password.");

        // Fetch meta info to get the name/title for the token payload
        var meta = await _db.UserMetaInfos.FindAsync(user.Id);
        var userName = meta?.Title ?? user.UserName;

        var expiryDate = DateTime.UtcNow.AddHours(24);
        var accessToken = _jwtService.IssueAccessToken(user.Id, user.Email, userName, "email_password");

        return ApiResponseDto<AuthResponseDto>.Success(new AuthResponseDto
        {
            AccessToken = accessToken,
            TokenType = "Bearer",
            ExpiresInSeconds = (int)(expiryDate - DateTime.UtcNow).TotalSeconds,
            User = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                UserName = user.UserName,
                CreatedAt = meta?.CreatedAt ?? DateTime.UtcNow
            }
        });
    }
}
