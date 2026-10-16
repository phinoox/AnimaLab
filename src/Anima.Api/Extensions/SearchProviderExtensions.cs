using System.Reflection;
using Anima.Api.ComponentServices.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace Anima.Api.Extensions;

public static class SearchProviderExtensions
{
    /// <summary>
    /// Automatically registers all classes implementing ISearchableProvider in the specified assembly.
    /// </summary>
    /// <param name="services">The service collection to add search providers to.</param>
    /// <param name="assembly">The assembly to scan for implementations.</param>
    /// <returns>The updated <see cref="IServiceCollection"/>.</returns>
    public static IServiceCollection AddSearchProviders(this IServiceCollection services, Assembly assembly)
    {
        var providerTypes = assembly.GetTypes()
            .Where(t => t.IsClass && !t.IsAbstract && typeof(ISearchableProvider).IsAssignableFrom(t));

        foreach (var type in providerTypes)
        {
            services.AddScoped(typeof(ISearchableProvider), type);
        }

        return services;
    }
}
