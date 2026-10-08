# Base Services Guide: The Foundation of Domain Logic

Base services provide the foundational templates and shared logic that power the entire Anima API. They encapsulate common patterns like permission checking, transaction management, and slug-based entity resolution.

## 🏗️ Hierarchy of the Base Layer

The base layer is organized into three distinct tiers to accommodate different levels of complexity:

1. **The Core Foundation (`DomainService`)**
2. **The Metadata Specialist (`EntityDomainService<TMeta, TEntity>`)**
3. **The Lightweight Worker (`SimpleDomainService<TEntity>`)**

---

## 🧬 1. The Core Foundation: `DomainService`

All domain-level services inherit from `DomainService`. This is the most fundamental layer and provides access to cross-cutting concerns via the `ICoreServicesProvider`.

* **Purpose:** To provide a unified base for all services, offering access to logging, auditing, and basic authorization helpers.
* **Key Responsabilities:** 
    * Access to `ICoreServicesProvider` (Audit, Permissions, Mapper).
    * Providing common permission check wrappers like `CheckAccessAsync`.
    * Managing the current user's identity (`_userId`).

---

## 🧬 2. The Metadata Specialist: `EntityDomainService<TMeta, TEntity>`

For entities that are "anchored" to a project and have associated metadata (the "Soul"), use `EntityDomainService`.

* **Purpose:** To automate the boilerplate of slug-based resolution, transaction management, and permission checks for complex entities.
* **Key Capabilities:**
    * **Slug Resolution:** Automatically resolves `projectIdentifier` and `entityIdentifier` into GUIDs using the `ISlugResolverService`.
    * **Transaction Orchestration:** Manages database transactions automatically during creation/updates.
    * **Permission Integration:** Integrates permission checks directly into the CRUD workflow.
* **The Implementation Pattern (Hooks):** 
  Concrete services implement "Hooks" rather than full methods to focus on business logic:
  * `OnCreateAsync(...)`
  * `OnUpdateAsync(...)`
  * `OnDeleteAsync(...)`

---

## 🧬 3. The Lightweight Worker: `SimpleDomainService<TEntity>`

For entities that are simpler or do not require slug-based resolution/complex metadata, use `SimpleDomainService`.

* **Purpose:** To provide a streamlined CRUD template for basic domain objects.
* **Key Capabilities:**
    * Standardized update and delete implementations with built-in error handling and logging.
    * Minimal boilerplate compared to the Metadata specialist.

---

## ⚖️ Decision Matrix: Which Base Service to use?

Use this matrix to decide which class to inherit from when building a new service.

| Feature / Requirement | `DomainService` | `SimpleDomainService<T>` | `EntityDomainService<TMeta, T>` |
| :--- | :---: | :---: | :---: |
| **Need Access to Core Services (Audit/Permissions)?** | ✅ Yes | ✅ Yes | ✅ Yes |
| **Managing simple entities by GUID?** | ❌ No | ✅ Yes | ❌ No |
| **Managing entities via Slugs?** | ❌ No | ❌ No | ✅ Yes |
| **Entity has complex Metadata (Soul)?** | ❌ No | ❌ No | ✅ Yes |
| **Requires Automatic Transaction Management?** | ❌ No | ❌ No | ✅ Yes |

---

## 🛠️ Implementation Flow for `EntityDomainService`

Because of the heavy lifting done in the base class, implementing a service follows a "Hook" pattern rather than a full implementation:

1.  **Inherit**: Extend `EntityDomainService<TMeta, TEntity>`.
2.  **Implement Hooks**: Override the abstract methods (`OnCreateAsync`, etc.).
3.  **Focus on Logic**: Your implementation only needs to focus on the *actual* data operations and business rules, while the base class handles the "plumbing" (permissions, slugs, transactions).

---
