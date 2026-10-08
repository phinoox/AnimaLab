# ServiceCollectionExtensions

The `ServiceCollectionExtensions` class provides an automated way to register all domain services into the .NET Dependency Injection (DI) container. It uses reflection to scan for classes inheriting from `DomainService` and registers them with their corresponding interfaces.

## 1. Purpose & Automation

Manual registration of every service in a growing project is error-prone and creates significant maintenance overhead. This extension method automates that process by following a standardized naming convention and applying custom lifetime metadata via attributes.

## 2. Registration Logic

When `AddDomainServices` is called, it performs the following steps:

### A. Assembly Scanning
The system scans the assembly containing the `DomainService` base class to find all concrete (non-abstract) classes that inherit from it.

### B. Lifetime Management
Instead of assuming a single lifetime for all services, it looks for a custom `[ServiceLifetime]` attribute on each implementation.
* **Custom Lifetime**: If the attribute is present, the service is registered with the specified lifetime (e.g., Singleton or Transient).
* **Default Lifetime**: If no attribute is found, the service defaults to `Scoped`.

### C. Interface Mapping
The extension identifies all interfaces implemented by the class that follow the standard `"I"` naming convention (e.g., `IProjectService`). It then registers the mapping:
`services.Add(new ServiceDescriptor(interfaceType, implementationType, lifetime));`

### D. Concrete Type Registration
To support direct injection of the concrete implementation (e.g., `AddScoped<ProjectService, ProjectService>()`), it also registers the class itself in the DI container.

---

## 3. Implementation Example

If you have a service defined as follows:

```csharp
[ServiceLifetime(ServiceLifetime.Transient)] // Custom lifetime
public class TaskService : ITaskService, IAnotherInterface
{
    // ... implementation
}
```

Calling `services.AddDomainServices();` will automatically perform the following registrations:
1. `services.AddTransient<ITaskService, TaskService>();`
2. `services.AddTransient<IAnotherInterface, TaskService>();`
3. `services.AddTransient<TaskService, TaskService>();`

---

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Low Maintenance**: Adding a new service requires zero changes to the startup/DI configuration as long as it inherits from `DomainService`. | **Reflection Overhead**: Scanning assemblies via reflection at startup adds a small amount of time to application initialization. |
| **Consistency**: Enforces a standardized way of registering services and managing their lifetimes across the entire domain. | **Magic Registration**: Since registration happens "under the hood," it can be harder for new developers to trace exactly how a service was registered without looking at the extension method. |

***
*Last Updated: [Date]*