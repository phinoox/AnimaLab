# Authorization System

The GaDeMa authorization system is a multi-layered mechanism designed to ensure that users can only access resources and perform actions within their permitted scope. It moves away from simple, global role checks toward a more granular **Capability-Based Authorization** model.

Authorization in GaDeMa is not just about *who* you are (Identity), but *where* you are acting (Scope).

---

## 1. The Four-Gate Flow

Every request passing through the system must clear four distinct security gates to be considered authorized for execution.

### Gate 1: Visibility Gate
The first check determines if a resource is even visible to the requester.
* **Logic**: Checks the `IsPublic` flag on the target entity.
* **Outcome**: If `true`, access is granted for viewing only. If `false`, the request proceeds to the next gate.

### Gate 2: Identity Gate
Confirms that a valid identity exists for the current request.
* **Logic**: Uses the `IUserContext` to verify authentication.
* **Outcome**: If no authenticated user is found and the resource is private $\rightarrow$ **Denied (Unauthorized)**.

### Gate 3: Authorization Gate
The most critical layer, where granular permissions are evaluated against a specific scope.
* **Logic**: Calls the `IPermissionEngine` to evaluate if the current user has the required capability for the target resource.
* **Scope-Awareness**: This check is context-aware (e.g., *"Does this user have 'Editor' rights specifically within Project X?"*).
* **Outcome**: If permissions are insufficient $\rightarrow$ **Denied (Forbidden)**.

### Gate 4: Execution Gate
The final gate where the actual business logic is performed.
* **Logic**: Once all previous gates pass, the `DomainService` executes the requested operation and may trigger an identity synchronization (Harmonization) if the action modifies the resource's core identity.

---

## 2. The Permission Engine (`IPermissionEngine`)

The `IPermissionEngine` acts as the "Brain" of the authorization system. It does not hold hardcoded rules; instead, it orchestrates decision-making by iterating through pluggable **Permission Strategies**.

### Core Mechanism
1.  **Input**: Receents a User ID, a Project/Scope ID, and a required Permission (e.g., `CanEdit`).
2.  **Strategy Evaluation**: The engine iterates through all registered `IPermissionStrategy` implementations.
3.  **Decision**: If any strategy returns an `Allowed` result, the request proceeds.

### The Result Object: `AccessResult`
The engine returns a lightweight `AccessResult` record rather than throwing exceptions, allowing for clean, predictable error handling in the service layer.

```csharp
public record AccessResult(AccessResultStatus Status, string Message = "");
```

---

## 3. Role Resolution & Mapping

To bridge the gap between high-level permissions and specific user roles, GaDeMa implements a **Role Resolution** pattern within the `DomainService`.

When a developer calls `CheckAccessAsync` with an abstract permission (e.g., `Permission.CanEdit`), the service automatically maps that permission to the minimum required role for that context:

| Requested Permission | Resolved Minimum Role |
| :--- | :--- |
| `Permission.CanDelete` | `ProjectMemberRoleEnum.Admin` |
| `Permission.CanCreate` | `ProjectMemberRoleEnum.Admin` |
| `Permission.CanEdit` | `ProjectMemberRoleEnum.Editor` |
| `Permission.CanView` | `ProjectMemberRoleEnum.Viewer` |
| *Default/Unmapped* | `ProjectMemberRoleEnum.Owner` |

This mapping allows the API to be intuitive for consumers while maintaining strict, role-based enforcement in the background.

---

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Granularity**: Allows for highly specific permissions that are scoped to individual projects or teams. | **Complexity**: Requires careful management of roles and permission mappings across different domains. |
| **Decoupling**: The `PermissionEngine` is agnostic of the specific business logic, making it highly reusable. | **Performance Overhead**: Multi-gate checks and strategy iteration add slight latency to request processing. |

***
*Last Updated: [Date]*
