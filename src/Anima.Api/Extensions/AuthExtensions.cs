using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Anima.Api.Extensions;

/// <summary>
/// Provides extension methods for configuring authentication and authorization in the application.
/// </summary>
public static class AuthExtensions
{
    /// <summary>
    /// Configures JWT Bearer authentication and adds authorization services to the service collection.
    /// </summary>
    /// <param name="services">The service collection to add authentication/authorization to.</param>
    /// <param name="configuration">The application configuration containing JWT settings.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    /// <exception cref="InvalidOperationException">Thrown when the JWT secret is missing or too short (less than 32 bytes).</exception>
    public static IServiceCollection AddGademaAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var jwtSecret = configuration["Jwt:Secret"];
        if (string.IsNullOrWhiteSpace(jwtSecret) || Encoding.UTF8.GetByteCount(jwtSecret) < 32)
        {
            throw new InvalidOperationException("Jwt:Secret must be set and at least 32 bytes long.");
        }

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret!)),
                    ValidateIssuer = true,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "GaDeMa",
                    ValidateAudience = true,
                    ValidAudience = configuration["Jwt:Audience"] ?? "GademaApi",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                };
            });

        services.AddAuthorization();
        return services;
    }
}