using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Anima.Api.Utils;

/// <summary>
/// Issues and validates JWTs.
/// Two kinds:
///  • Access token  – full session (auth_method, sub, email, name), 1 h
///  • Pending token – 2FA step 2 / setup confirmation, 8 min, `purpose` claim
/// </summary>
public class JwtTokenService
{
    private readonly IConfiguration _config;
    private readonly byte[] _key;

    public JwtTokenService(IConfiguration configuration)
    {
        _config = configuration;
        _key = Encoding.UTF8.GetBytes(configuration["Jwt:Secret"]
            ?? throw new InvalidOperationException("Jwt:Secret is required in configuration."));
    }

    public string IssueAccessToken(Guid userId, string email, string? name, string authMethod)
        => Write(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim(JwtRegisteredClaimNames.Email, email),
            new Claim(JwtRegisteredClaimNames.Name, name ?? ""),
            new Claim("auth_method", authMethod),
        }, TimeSpan.FromHours(1));

    public string IssuePendingToken(Guid userId, string purpose)
        => Write(new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId.ToString()),
            new Claim("purpose", purpose),     // "2fa_signin" | "2fa_setup"
        }, TimeSpan.FromMinutes(8));

    public (bool Valid, Guid? UserId, string? Purpose) Validate(string token)
    {
        try
        {
            var handler = new JwtSecurityTokenHandler();
            var claimsPrincipal = handler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(_key),
                ValidateIssuer = true,
                ValidIssuer = _config["Jwt:Issuer"] ?? "AnimaLab",
                ValidateAudience = true,
                ValidAudience = _config["Jwt:Audience"] ?? "AnimaApi",
                ValidateLifetime = true,
                ClockSkew = TimeSpan.FromSeconds(30),
            }, out var validated);

            var userIdClaim = claimsPrincipal.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub);
            Guid? userId;
            if ((userIdClaim != null) && Guid.TryParse(userIdClaim.Value, out var g))
            {
                userId = g;
            }
            else
            {
                userId = null;
            }

            return (true, userId, claimsPrincipal.Claims.FirstOrDefault(c => c.Type == "purpose")?.Value);
        }
        catch
        {
            return (false, null, null);
        }
    }

    private string Write(Claim[] claims, TimeSpan lifetime)
    {
        var credentials = new Microsoft.IdentityModel.Tokens.SigningCredentials(
            new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(_key),
            Microsoft.IdentityModel.Tokens.SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _config["Jwt:Issuer"] ?? "AnimaLab",
            audience: _config["Jwt:Audience"] ?? "AnimaApi",
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: DateTime.UtcNow + lifetime,
            signingCredentials: credentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}