# Orchestration & Domain Services

The GaDeMa architecture uses an orchestration layer to manage how different specialized engines interact with business logic. This is achieved through a combination of the `CoreServicesProvider` (the Gateway) and the `DomainService` (the Base implementation).

This pattern ensures that domain-specific logic remains focused on "what" to do, while the infrastructure handles the "how" of auditing, authorization, and identity synchronization.

---

## 1. The Gateway: `CoreServicesProvider`

The `CoreServicesProvider` acts as a central registry for all cross-cutting infrastructure engines. It implements the **Parameter Object** (or Gateway) pattern to prevent "Constructor Bloat" in domain services.

Instead of injecting five or six separate interfaces into every service, we inject this single provider which grants access to:

| Service | Responsibility |
| :--- | :--- |
| `IAuditService` | Records business-level activity logs and audit trails. |
| `IMetadataService` | Manages the lifecycle and synchronization of `MetaInfo` anchors. |
| `IPermissionEngine` | Evaluates granular, scope-based permissions via strategies. |
| `IUserContext` | Provides identity (UserId, Roles) for the current request. |

---

## 2. The Base: `DomainService`

The `DomainService` is an abstract base class that all business logic implementations inherit from. It acts as the bridge between the raw infrastructure engines and the high-level API controllers.

### Role & Responsibility
A `DomainService` is responsible for implementing the actual "business rules" of a specific domain (e.g., managing tasks, handling characters). It uses the `CoreServicesProvider` to wrap complex engine logic into simplified, readable helper methods.

### Key Patterns Implemented

#### A. The Wrapper Pattern (Helper Methods)
To keep domain code clean and ensure consistent error responses, `DomainService` provides high-level wrappers for core operations:

* **Authorization Wrappers**: Maps `IPermissionEngine` results into standardized `ApiResponseDto<T>` objects. It also handles **Role Resolution**—automatically determining that a request for `CanEdit` requires the `Editor` role.
* **Auditing Helpers**: Provides a simple way to record events (e.g., `LogDbAsync`) without exposing the complexity of the audit engine.
* **Harmonization Wrappers**: Provides access to the `IMetadataService.SyncAsync` method, allowing domain services to trigger identity updates through the Harmonization protocol.

#### B. The Nexus Pattern (Scene-Driven State)
In the context of narrative content, the `DomainService` manages the `Scene`, which acts as a **Nexus**. This is where static narrative structures (Beats/Sequences) intersect with dynamic character states and relations.

---

## 3. Interaction Flow

The interaction between these components follows a structured flow:

1.  **Request**: An API Controller calls a method on a `DomainService`.
2.  **Authorization**: The `DomainService` uses the `_core.PermissionEngine` (via its internal wrapper) to check if the user has permission.
3.  **Execution**: If authorized, the service performs its primary business logic (e.g., updating a character's stats).
4.  **Harmonization**: If the change affects a core identity property, the `DomainService` calls `SyncIdentityAsync<T>` to ensure the `MetaInfo` anchor is synchronized.
5.  **Auditing**: Finally, the service records the action via `LogDbAsync`.

---

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Reduced Boilerplate**: Dramatically simplifies service constructors and error handling. | **Inheritance Coupling**: All domain services are tightly bound to the `DomainService` base class. |
| **Standardized API**: Ensures that all authorization checks and audit logs follow a consistent format across the entire system. | **Abstraction Overhead**: Can make it harder for developers to see exactly which underlying engine is being called without stepping into the code. |

***
*Last Updated: [Date]*
