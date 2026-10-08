# API Infrastructure Guide: Extensions & Middleware

This guide explains how the Anima API handles core infrastructure tasks such as authentication configuration, dependency injection registration, and identity hydration through middleware.

## 🛠️ Extension Methods

Extension methods are used to encapsulate complex setup logic, keeping `Program.cs` clean and providing a fluent API for configuring different parts of the application.

### 1. Authentication Configuration (`AuthExtensions`)
The `AddGademaAuthentication` extension method centralizes the configuration of JWT Bearer authentication. It handles:
*   Validating the presence and length of the `Jwt:Secret`.
*   Setting up `TokenValidationParameters` (Issuer, Audience, Lifetime, etc.).
*   Adding Authorization services to the container.

### 2. Automated Service Registration (`ServiceCollectionExtensions`)
To reduce boilerplate code, we use a reflection-based registration mechanism. The `AddDomainServices` extension method scans the assembly for any class inheriting from `DomainService`.

**Key Features:**
*   **Automatic Interface Mapping**: It automatically registers implementation classes against all interfaces they implement (e.g., `ProjectService` $\rightarrow$ `IProjectService`).
*   **Custom Lifetime Control**: By default, services are registered as `Scoped`. However, you can override this by decorating a class with the `[ServiceLifetime]` attribute.

```csharp
[ServiceLifetime(ServiceLifetime.Singleton)]
public class MySingletonService : DomainService { ... }
```

---

## 🛡️ Middleware: Identity Hydration

The `AuthMiddleware` is a critical component in the request pipeline, responsible for "hydrating" the user context.

### The Role of `AuthMiddleware`
Instead of manually extracting user information in every controller or service, the middleware automs this process at the start of every authenticated request.

**How it works:**
1.  **Intercepts Request**: CaptGet the `ClaimsPrincipal` from the current HTTP context.
2.s **Extracts Identity**: Parses the `NameIdentifier` claim to retrieve the User ID.
3.  **Hydrates Context**: Injects the user's identity and roles into the scoped `IUserContext`.
4.  **Lazy-loads Entity**: Fetches the full `User` entity from the database, ensuring that downstream services have access to complete user information without repeated DB calls.

---

## 🔄 The Request Lifecycle

The following diagram illustrates how a request flows through these infrastructure components:

1.  **HTTP Request arrives** $\rightarrow$
2.  **Authentication Middleware** (Standard ASP.NET Core) verifies the JWT signature $\rightarrow$
3.  **`AuthMiddleware`** (Anima Custom):
    *   Extracts User ID from Claims.
    *   Fetches `User` entity from DB.
    *   Populates the scoped `IUserContext`.
4.  **Routing/Controller**: The request reaches your controller.
5.  **Dependency Injection**: The controller (or service) receives the already-populated `IUserContext` via constructor injection.
6.  **Business Logic**: Services can now immediately access `userContext.CurrentUser` or `userContext.Roles` without extra logic.

## 📝 Summary Table

| Component | Type | Primary Responsibility |
| :--- | :--- | :--- |
| `AuthExtensions` | Extension | Configuring JWT and Security policies. |
| `ServiceCollectionExtensions` | Extension | Automating DI registration for Domain Services. |
| `AuthMiddleware` | Middleware | Hydrating `IUserContext` from incoming claims. |
| `UserContext` | Scoped Object | Holding the identity state for the duration of a request. |
