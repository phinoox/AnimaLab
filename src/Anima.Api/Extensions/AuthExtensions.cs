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
    public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration configuration)
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
                    ValidateIssuer = false,
                    ValidIssuer = configuration["Jwt:Issuer"] ?? "AnimaLab",
                    ValidateAudience = false,
                    ValidAudience = configuration["Jwt:Audience"] ?? "AnimaApi",
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.FromSeconds(30),
                };
                options.Events = new JwtBearerEvents
                {
                    OnAuthenticationFailed = context =>
                    {
                        // This will tell you exactly WHY the token was rejected
                        Console.WriteLine($"[AUTH ERROR] {context.Exception.Message}");
                        return Task.CompletedTask;
                    },
                    OnTokenValidated = context =>
                    {
                        Console.WriteLine("[AUTH SUCCESS] Token validated successfully.");
                        return Task.CompletedTask;
                    },
                    OnMessageReceived = context =>
                    {
                        Console.WriteLine($"[AUTH Message] Received token {context.Request.Query["access_token"]}.");
                        return Task.CompletedTask;
                    },
                    OnChallenge = context =>
                    {
                        Console.WriteLine($"[AUTH Challenge] statuscode was {context.Response.StatusCode}.");
                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        Console.WriteLine($"[AUTH Forbidden] statuscode was {context.Response.StatusCode}.");
                        return Task.CompletedTask;
                    }
                };
            });

        services.AddAuthorization();
        return services;
    }
}