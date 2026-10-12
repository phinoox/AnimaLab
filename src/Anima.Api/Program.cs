using Anima.Api.CoreServices;
using Anima.Api.CoreServices.Interfaces;
using Anima.Api.CoreServices.Services;
using Anima.Api.Extensions;
using Anima.Api.Infrastructure.Commands;
using Anima.Api.MiddleWare;
using Anima.Api.Utils;
using Anima.Core.Base.Commands;
using Anima.Core.Base.Interfaces;
using Anima.Data.Contexts;
using Microsoft.EntityFrameworkCore;

using Scalar.AspNetCore;

namespace Anima.Api;

public partial class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // 1. Standard Services
        builder.Services.AddControllers();
        builder.Services.AddHttpContextAccessor();

        // 2. Database Configuration
        builder.Services.AddDbContext<AppDbContext>(options =>
            options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

        // 3. Core Services (The Gateway)
        builder.Services.AddScoped<ICoreServicesProvider, CoreServicesProvider>();
        builder.Services.AddScoped<IPermissionEngine, PermissionEngine>();
        builder.Services.AddScoped<IAuditService, AuditService>();
        builder.Services.AddScoped<IUserContext,UserContext>();

        builder.Services.AddScoped<IEntityMapper,EntityMapper>();
        builder.Services.AddScoped<ISlugResolverService,SlugResolverService>();
        
        // 4. Identity & Security
        builder.Services.AddSingleton<JwtTokenService>();
        //builder.Services.AddScoped<IIdentitySyncStrategy, UserIdentityStrategy>();

        // 5. Automatic Command Handler Registration via Reflection
        RegisterHandlers(builder.Services);

        // 6. Command Bus Registration
        builder.Services.AddScoped<ICommandBus, CommandBus>();
        builder.Services.AddJwtAuthentication(builder.Configuration);

        // 7. OpenAPI Configuration
        builder.Services.AddOpenApi();

        var app = builder.Build();

        // 8. Middleware Pipeline
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.MapScalarApiReference();
        }
        else
        {
            app.UseExceptionHandler("/error");
            app.UseHsts();
        }

        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseMiddleware<AuthMiddleware>();
        app.MapControllers();
        app.MapGet("/health", () => Results.Ok(new { Status = "healthy" }));

        app.Run();
    }

    /// <summary>
    /// Scans the assembly for all implementations of ICommandHandler<,> and registers them in DI.
    /// </summary>
    private static void RegisterHandlers(IServiceCollection services)
    {
        var assembly = typeof(Program).Assembly;
        var handlerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.GetInterfaces().Any(i => 
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>)));

        foreach (var handlerType in handlerTypes)
        {
            var interfaceType = handlerType.GetInterfaces().First(i => 
                i.IsGenericType && i.GetGenericTypeDefinition() == typeof(ICommandHandler<,>));
            
            services.AddScoped(interfaceType, handlerType);
        }
    }
}