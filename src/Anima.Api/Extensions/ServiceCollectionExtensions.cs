using System.Reflection;
using Anima.Api.Base;
using Microsoft.Extensions.DependencyInjection;

namespace Anima.Api.Extensions;

/// <summary>
/// Provides extension methods for the <see cref="IServiceCollection"/> to facilitate automated service registration.
/// </summary>
public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Automatically registers all classes inheriting from <see cref="DomainService"/> in the same assembly.
    /// The lifetime of each service is determined by the presence of a <see cref="ServiceLifetimeAttribute"/>; 
    /// otherwise, it defaults to <see cref="ServiceLifetime.Scoped"/>.
    /// </summary>
    /// <param name="services">The service collection to add domain services to.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        // Scan the assembly containing DomainService
        var serviceAssembly = typeof(DomainService).Assembly;

        var serviceTypes = serviceAssembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(DomainService)));

        foreach (var implementationType in serviceTypes)
        {
            // 1. Determine Lifetime via Attribute or default to Scoped
            var lifetimeAttr = implementationType.GetCustomAttribute<ServiceLifetimeAttribute>();
            var lifetime = lifetimeAttr?.Lifetime ?? ServiceLifetime.Scoped;

            // 2. Find all interfaces (e.g., IProjectService) that follow the "I" naming convention
            var serviceInterfaces = implementationType.GetInterfaces()
                .Where(i => i.Name != nameof(DomainService) && i.Name.StartsWith("I"));

            // 3. Register each interface mapping: services.AddScoped<IInterface, Implementation>()
            foreach (var interfaceType in serviceInterfaces)
            {
                services.Add(new ServiceDescriptor(interfaceType, implementationType, lifetime));
            }

            // 4. Also register the concrete type itself to support AddScoped<ProjectService, ProjectService>()
            services.Add(new ServiceDescriptor(implementationType, implementationType, lifetime));
        }

        return services;
    }
}