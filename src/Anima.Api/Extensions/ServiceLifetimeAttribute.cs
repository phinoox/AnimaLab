// src/Gadema.Api/Services/Core/ServiceLifetimeAttribute.cs
namespace Anima.Api.Extensions;

[AttributeUsage(AttributeTargets.Class)]
public class ServiceLifetimeAttribute : Attribute
{
    public ServiceLifetime Lifetime { get; }
    public ServiceLifetimeAttribute(ServiceLifetime lifetime) => Lifetime = lifetime;
}