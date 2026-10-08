namespace Anima.Api.Extensions;

/// <summary>
/// Specifies the dependency injection lifetime for a service class.
/// This attribute is used by the automatic service registration mechanism in <see cref="ServiceCollectionExtensions"/>.
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
public class ServiceLifetimeAttribute : Attribute
{
    /// <summary>
    /// Gets the specified dependency injection lifetime.
    /// </summary>
    public ServiceLifetime Lifetime { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceLifetimeAttribute"/> class with the specified lifetime.
    /// </summary>
    /// <param name="lifetime">The dependency injection lifetime to apply to the decorated class.</param>
    public ServiceLifetimeAttribute(ServiceLifetime lifetime) => Lifetime = lifetime;
}