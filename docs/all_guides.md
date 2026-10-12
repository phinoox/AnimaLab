==> guides/architecture/api-infrastructure.md <==
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

==> guides/architecture/base-services.md <==
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

==> guides/architecture/component-services.md <==
# Component Services Guide

Component Services in the Anima architecture are specialized, granular services designed to handle specific domain responsibilities. They act as "sub-resource workers" (the "Muscle") that are orchestrated by higher-level Domain Services to complete complex business workflows.

## 🎯 Purpose and Role

While **Domain Services** manage high-level business processes (e.g., "Creating a Project"), **Component Services** handle the specific, low-level details of individual sub-entities or features (e.g., "Adding a Tag" or "Updating a Comment status").

In the service hierarchy, they sit between **Core Services** and **Domain Services**:

1.  **Domain Services** (Orchestrators)
2.  **Component Services** (Specialized Workers) $\leftarrow$ *You are here*
3.  **Core Services** (Cross-cutting Infrastructure)

## 🛠️ Key Characteristics

### 1. Granularity
Each component service should focus on a single, well-defined responsibility. They do not attempt to manage the entire domain; instead, they manage one specific aspect of it.

*   `TagService`: Manages tag creation and the relationship between tags and content.
*   `CommentService`: Manages the lifecycle and hierarchy of comments.
*   `ReviewStatusService`: Manables the review status state for content items.

### 2. Composition (The "Muscle" Pattern)
Component services are designed to be composed. A `DomainService` typically consumes multiple `ComponentServices` via the `IComponentServiceProvider`.

**Example Workflow:**
To perform a complex operation like "Publishing a Content Item," a `ContentDomainService` might:
1.  Call `PermissionEngine` (Core) to check write access.
2.  Call `ReviewStatusService` (Component) to set the status to 'Published'.
3.  Call `TagService` (Component) to sync associated tags.
4.  Call `AuditService` (Core) to log the publication event.

### 3. Project Context Awareness
Most component services operate within a project scope. They typically require a `projectId` in their method signatures to ensure operations are performed within the correct boundary and to facilitate permission checks.

## 🏗️ Implementation Pattern

When adding a new component to the system, follow this standardized workflow:

### Step 1: Define the Interface
Create an interface in `refactored/src/Anima.Api/ComponentServices/Interfaces/`. The interface should clearly define the granular operations available for that component.

```csharp
public interface ITagService 
{
    Task<ApiResponseDto<IdentityDto>> AddTagAsync(Guid projectId, Guid targetId, string name);
    // ... other specific methods
}
```

### Step 2: Implement the Service
Implement the logic in `refactored/src/Anima.Api/ComponentServices/Services/`. 

*   Inherit from `DomainService` to gain access to `ICoreServicesProvider` and logging.
*   Use `AppDbContext` for data persistence.
*   Perform permission checks via the injected `coreServices.PermissionEngine`.
*   Wrap operations in transactions where multiple database changes are required.

### Step 3: Register the Service
Add the new service to the `IComponentServiceProvider` interface and its implementation, `ComponentServiceProvider`, so it can be easily accessed by Domain Services.

## 📝 Summary Table

| Feature | Component Service | Domain Service |
| :--- | :--- | :--- |
| **Scope** | Granular (Single Entity/Feature) | Broad (Business Workflow) |
| **Responsibility** | "How to do a specific thing" | "How to achieve a business goal" |
| **Orchestration** | Is orchestrated by Domain Services | Orchestrates Component & Core services |
| **Complexity** | Low to Medium | High |

==> guides/architecture/services-architecture.md <==
# Service Architecture Guide

The Anima architecture employs a tiered service pattern to ensure a strict separation of concerns, scalability, and maintainability. Services are organized into four distinct layers based on their scope and responsibility.

## 🏗️ Hierarchy Overview

The hierarchy flows from low-level infrastructure/foundation up to high-level business logic:

1. **Base Services** (Foundation)
2. **Core Services** (Cross-cutting Concerns)
3. **Component Services** (Specific Domain Components)
4. **Domain Services** (High-level Business Logic)

---

## 1. Base Services (The Foundation)
Located in: `refactored/src/Anima.Api/Base/`

Base services provide the fundamental templates and abstract logic used by all subsequent service layers. They define the standard patterns for CRUD operations, entity management, and repository interactions.

* **Purpose:** To standardize how entities are managed and how domain logic is structured.
* **Key Classes:** `DomainService`, `EntityDomainService`, `SimpleDomainService`.
* **Responsibility:** Implementing common patterns like soft deletion, entity retrieval by ID, and basic persistence orchestration.

## 2. Core Services (Cross-cutting Concerns)
Located in: `refactored/src/Anima.Api/CoreServices/`

Core services provide infrastructure-level functionality that is required across many different parts of the system. They are "cross-cutting," meaning they don't belong to a single domain but support the entire application.

* **Purpose:** To handle systemic tasks like security, auditing, and data transformation.
* **Key Services:** 
    * `AuditService`: Logging activity and audit trails.
    * `PermissionEngine`: Evaluating authorization and role-based access.
    * `EntityMapper`: Mapping DTOs to Entities with attribute-aware logic.
    * `SlugResolverService`: Resolving URL-friendly identifiers.
* **Access:** Typically accessed via the `ICoreServicesProvider`.

## 3. Component Services (Domain Components)
Located in: `refactored/src/Anima.Api/ComponentServices/`

Component services are specialized services that manage specific, often smaller, pieces of domain logic. They focus on a single responsibility within a larger context.

* **Purpose:** To encapsulate the logic for specific sub-entities or features.
* **Examples:** `CommentService`, `TagService`, `ReviewStatusService`.
* **Responsibility:** Managing the lifecycle and business rules of a particular component (e.g., "How is a tag attached to content?").

## 4. Domain Services (Business Logic)
Located in: `refactored/src/Anima.Api/DomainServices/`

Domain services are the highest level of service logic. They orchestrate complex business workflows that may involve multiple components, core services, and base services.

* **Purpose:** To implement the primary business rules and workflows of the application.
* **Examples:** `ProjectService`, `BlogService`, `EmailPasswordAuthService`.
* **Responsibility:** Managing high-level operations (e.g., "Creating a new project," "Processing a blog post submission") by orchestrating various underlying services.

---

## 🔄 Request Flow Example

When an API request is made to create a comment on a piece of content, the flow typically looks like this:

1. **API Controller** receives the request.
2. **Domain Service** (e.g., `ProjectService`) orchestrates the high-level operation.
3. **Component Service** (e.g., `CommentService`) handles the specific logic of creating the comment entity.
4. **Core Services** (e.g., `PermissionEngine`) are called to verify if the user is allowed to comment.
5. **Core Services** (e.g., `AuditService`) are called to log the action.
6. **Base Services** (e.g., `EntityDomainService`) facilitate the actual database persistence via the repository/context.

## 🛠️ Implementation Rules

* **Dependency Flow:** Higher-level services can depend on lower-level services, but a lower-level service should **never** depend on a higher-level one.
* **Core Services are Universal:** Any service in the system may (and often should) utilize Core Services for cross-cutting concerns.
* **Interfaces First:** Always define the contract in the `Interfaces` folder before implementing the logic in `Services`.

==> guides/basics/core-identity-pattern.md <==

***

# 🧬 Core Architecture: The Identity/Container Pattern

Anima uses a **Body-Soul** architecture to decouple an entity's stable identity from its evolving structural data. This separation ensures that as complexity grows, the core identity remains stable and predictable.

## 1. The Two Types of Entities

Every object in Anima is categorized by whether it possesses a "Soul" (descriptive metadata) or just a "Body" (a structural identifier).

### Type A: Base-Entities (Body Only)
A **Base-Entity** is an object that requires a database presence but does not require descriptive metadata. It acts as a structural anchor or a lightweight identifier. 
*   **Requirement**: Must inherit from `EntityBase`.
*   **Examples**: `ProjectMember`, `Comment`, `ActivityLog`.

```C#
public abstract class EntityBase
{
    /// <summary>
    /// Gets or sets the unique identifier for the entity.
    /// </summary>
    [Key]
    public Guid Id {get;init;}
}

```


### Type B: Meta-Entities (Body + Soul)
A **Meta-Entity** is an independent object that carries rich, descriptive metadata. It has a "Soul" used for identity, routing, and search.
*   **Requirement**: Must inherit from `MetaEntityBase<TMeta>`.
*   **Examples**: `User`, `Project`, `Character`, `Scene`.

```C#

public abstract class MetaEntityBase<TMeta> : EntityBase where TMeta : BaseMetaInfo
{
    /// <summary>
    /// Gets or sets the rich metadata associated with this entity.
    /// </summary>
    [ForeignKey("Id")]
    public virtual TMeta MetaInfo { get; set; } = null!;
}
```

---

## 2. The Hierarchy of Identity (The Soul)

The "Soul" follows a branching hierarchy. These base classes are `abstract` and serve as templates for building concrete metadata models. Depending on whether your entity represents a root object or something contained within it, you will follow a different lineage.

### The Branching Lineage

All souls originate from `BaseMetaInfo`. From there, the hierarchy branches into **Global Identities**, **Root Entities**, and **Scoped Content**.

| Level | Base Class (Abstract) | Concrete Implementation Example | Context/Scope Added |
| :--- | :--- | :--- | :--- |
| **Universal** | `BaseMetaInfo` | `UserMetaInfo` | Core identity: `Id`, `Title`, and auto-generated `Slug`. |
| **Root** | BaseMetaInfo| `ProjectMetaInfo` | The identity of the Project itself (the top-level container). |
| **Scoped** | `ScopedMetaInfo` | `ContentMetaInfo` | Adds a `ProjectId` to anchor content within a project. |

### 🧬 Visualizing the Hierarchy

To visualize how these souls are structured, look at this tree:

```text
BaseMetaInfo (Universal Identity)
├── Global Branch (e.g., UserMetaInfo)
└── Scoped Branch (via ScopedMetaInfo)
    └── Content (e.g., Character, Scene, Task)
```

### 🔍 How to choose your lineage:

* **"I am a User":** Use `UserMetaInfo` (inherits from `BaseMetaInfo`).
* **"I am the Project itself":** Use `ProjectMetaInfo` (inherits from `BaseMetaInfo`).
* **"I am something inside a Project":** 
    1. Create your custom soul by inheriting from `ScopedMetaInfo`.
    2. Or, if it is standard content (taggable/reviewable), use the existing `ContentMetaInfo`.




---

## 3. The Decision: Module vs. Entity

Before creating a class, apply the **Autonomy Test**. This determines if your object is an independent entity or just a component of another.

### Step 1: Is it an Independent Identity?
| If the object... | Use this Pattern | Implementation |
| :--- | :--- | :--- |
| Needs its own Slug, Searchability, and Tagging. | **Entity** | `MetaEntityBase<T>` or `EntityBase` |
| Is just a property/extension of another entity. | **Module** | Plain Class (no inheritance) |

### Step 2: Which Identity Type?
If it is an **Entity**, use this matrix to select the correct base class:

| If the object... | Use this Base Class | Resulting Hierarchy |
| :--- | :--- | :--- |
| Has no descriptive metadata. | `EntityBase` | `EntityBase` $\rightarrow$ **Body Only** |
| Needs identity (Title/Slug) but is global. | `BaseMetaInfo` | `BaseMetaInfo` $\rightarrow$ **Soul** |
| Is anchored to a Project. | `ProjectEntityMetaInfo` | `BaseMetaInfo` $\rightarrow$ `ProjectEntityMetaInfo` $\rightarrow$ **Soul** |
| Needs status, versioning, or view modes. | `ContentMetaInfo` | `ProjectEntityMetaInfo` $\rightarrow$ `ContentMetaInfo` $\rightarrow$ **Soul** |

---

## 4. Data Integrity: The Hardening Rules

To prevent "Identity Drift" and ensure structural integrity, all models must follow these rules:

| Property Type | Implementation Rule | Technical Mechanism |
| :--- | :--- | :--- |
| **Identity-Critical** (`Id`, `ProjectId`) | Immutable after creation | Use `init` keyword |
| **Mandatory Data** (e.g., `Title`) | Mandatory at construction | Use `required` modifier |
| **Structural Constraints** | Enforced on the model | Use Data Annotations (`[Required]`, etc.) |

---

## 💡 Summary Cheat Sheet

### 🛠️ Step-by-Step Implementation Flow:

1.  **Autonomy Test**: Is it a **Module** (plain class) or an **Entity** (independent)?
2.  **Identity Check**: Does the Entity need a Soul?
    *   No $\rightarrow$ Inherit `EntityBase`.
    *   Yes $\rightarrow$ Select the appropriate `MetaInfo` hierarchy.
3.  **Hardening**: Apply `init`, `required`, and Attributes to all properties.


==> guides/basics/data-layer-p1.md <==
That is a vital guiding principle. Even with the "Pointer vs. Payload" distinction, stating it as a fundamental rule prevents developers from accidentally bloating the `MetaInfo` layer with private data under the guise of "resolution."

I'll add it as the primary directive for the `MetaInfo` layer.

***

# 📝 Draft: Data Layer Guide (WIP)

## Pillar A: Identity & Key Patterns

### 1. The Unification Pattern
All `MetaEntityBase<T>` implementations must share an identical `Id` between the base entity and its metadata to guarantee a strict 1:1 relationship through unified IDs.

> [!TIP]  
> **The Golden Rule**: The `MetaInfo` layer is primarily intended for **public-facing information**. If a piece of data is not required for discovery, resolution, or visibility, it belongs in the `Main Entity`.

### 2. Property Placement Decision Matrix

| Category | Function (The "Why") | Target Layer | Implementation Example |
| :--- | :--- | :--- | :--- |
| **Discovery, Resolution & Visibility** | Used for finding, resolving, or providing context via *pointers*. | `MetaInfo` | `Slug`, `Status`, `OwnerId`, `ParentId`. |
| **Management & Structure** | Used for internal state and heavy data collections. | `Main Entity` | `Members`, `Settings`, `Attachments`. |
| **Safety & Security** | Holds the *payloads* (actual objects) or sensitive configuration requiring authorization. | `Main Entity` | `User` (Nav Property), `Permissions`, `AccessLevels`. |

> [!IMPORTANT]  
> **The Pointer vs. Payload Rule**: A pointer (e.g., `OwnerId`) is not a leak; it is a key. Security is enforced at the Service Layer by verifying the payload's status (`IsDeleted`, `IsPublic`) and authorization permissions when the key is used to access the actual object.

### 3. Key Constraints & Immutability
To maintain identity stability, the following constraints are required at the database level:

| Property Type | Database Constraint | Purpose |
| :--- | :--- | :--- |
| **Identity (`Id`)** | `PRIMARY KEY` | Ensures unique entity identification. |
| **Resolution/Ownership (`OwnerId`, etc.)** | `NOT NULL` / `INDEX` | Enables efficient identity-based resolution. |
| **Routing Identity (`Slug`)** | `UNIQUE INDEX` | Prevents identity collisions during resolution. |

***
This is a perfect way to illustrate the concept. A clear "Wrong vs. Correct" example makes the rule unmistakable.

Here is how I would present it in the guide:

***

### 4. The Mapping Rule: "Follow the Column"
To prevent schema errors, always configure a relationship in the `EntityTypeConfiguration` of the entity that **physically holds the Foreign Key (FK) column**.

**Scenario**: You want to navigate from `Project` $\rightarrow$ `User`, but the `UserId` is actually stored on the `ProjectMetaInfo` table.

#### ❌ WRONG (In `ProjectEntityTypeConfiguration.cs`)
*Attempting to map a relationship using a column that does not exist in this table.*

```csharp
public void Configure(EntityTypeBuilder<Project> builder)
{
    // ERROR: The 'UserId' column is NOT on the Project table! 
    // This will cause a runtime error or incorrect schema.
    builder.HasOne(p => p.User)
        .WithMany()
        .HasForeignKey(p => p.UserId); 
}
```

#### ✅ CORRECT (In `ProjectMetaInfoEntityTypeConfiguration.cs`)
*Mapping the relationship in the Soul, where the physical column actually resides.*

```csharp
public void Configure(EntityTypeBuilder<ProjectMetaInfo> builder)
{
    // 1. Link the Soul to its Body (The identity anchor)
    builder.HasOne(mi => mi.Project)
        .WithOne(p => p.ProjectMetaInfo)
        .HasForeignKey<ProjectMetaInfo>(mi => mi.Id);

    // 2. Link the Soul to the User (The pointer)
    // SUCCESS: The 'UserId' column physically exists in this table!
    builder.HasOne(mi => mi.User)
        .WithMany()
        .HasForeignKey(mi => mi.UserId); 
}
```

**Result**: Even though you mapped it in the `MetaInfo` configuration, you can still access the user via navigation properties like `myProject.ProjectMetaInfo.User`.

***

==> guides/basics/data-layer-p2.md <==
That makes perfect sense. `ISoftDeletable` isn't just a property; it's a mechanism for **Logical Deletion** that fundamentally changes how `DeleteBehavior` is interpreted in the database.

If an entity is soft-deleted, it still exists in the table, but its "existence" is toggled off via `IsDeleted`. This creates a critical distinction: we are no longer talking about physical row deletion, but rather **Lifecycle State Management**.

I will integrate the `ISoftDeletable` pattern into Pillar B to show how it interacts with cascading logic.

***

# 📝 Draft: Data Layer Guide (WIP)

## Pillar B: Relationship Mapping

This section defines how entities relate to one another and how those relationships are maintained during database operations.

### 1. Relationship Types

| Type | Pattern | Description | Example |
| :--- | :--- | :--- | :--- |
| **Direct Link** | `1:N` (One-to-Many) | A structural anchor where one entity owns many others. | `Project` $\rightarrow$ `ProjectMember` |
| **Junction Table** | `M:N` (Many-to-Many) | Used for shared, non-hierarchical associations. | `MetaInfo` $\leftrightarrow$ `Tags` |
| **Identity Link** | `1:1` (Unification) | The structural bond between a Body and its Soul. | `Project` $\leftrightarrow$ `ProjectMetaInfo` |

### 2. Lifecycle & Deletion Logic

We distinguish between **Physical Deletion** (removing rows from the DB) and **Logical Deletion** (toggling existence via `ISoftDeletable`).

#### **The Soft-Delete Pattern (`ISoftDeletable`)**
Entities implementing `ISoftDeletable` are not physically removed; their `IsDeleted` flag is toggled. 
*   **Application**: Used to preserve history and allow for "undelete" operations.
*   **Enforcement**: Global Query Filters must be applied in `AppDbContext` to ensure `IsDeleted == false` is the default view for all queries.

#### **The Deletion Modes**
When a parent entity is deleted (either physically or logically), the following behaviors determine what happens to its children:

| Mode | Mechanism | Use Case | Implementation |
| :--- | :--- | :--- | :--- |
| **Structural Cascade** | `DeleteBehavior.Cascade` | For entities that are part of a strict hierarchy and cannot exist without their parent. | If the Parent dies, the Child is physically purged. |
| **Historical Preservation** | `DeleteBehavior.SetNull` | For "side-car" data (attachments, tags) where we want to keep the record even if the source entity is removed. | If the Parent dies, the child's reference is nullified, but the child remains. |

### 3. The Junction Table Rule (M:N)
When implementing many-to-many relationships via junction tables:
1.  The junction table is a **Base-Entity** (Body Only).
2.  If the relationship is purely for tagging/metadata, use `DeleteBehavior.SetNull` on the navigation properties to ensure that deleting an entity doesn't trigger a cascading purge of its historical associations.

***

==> guides/basics/data-layer-p3.md <==
Since I don't have a pre-written draft for Pillar C, I am building it now based on the architectural principles we have established throughout this session.

I am structuring Pillar C to focus on how we turn our "Hardening Rules" (from the Core phase) into actual database constraints using the **Fluent API**.

***

# 📝 Draft: Data Layer Guide (WIP)

## Pillar C: Configuration & Integrity

This section defines how we translate architectural rules into database-level enforcement, ensuring that the schema remains a strict implementation of our models.

### 1. The Single Source of Truth
To maintain clarity and prevent configuration drift, we follow a tiered approach to constraints:

*   **The Model (Intent)**: Uses Data Annotations (`[Required]`, `[MaxLength]`) to define the *intent* of the property. This serves as documentation for developers and basic validation.
*   **The Configuration (Enforcement)**: Uses the **Fluent API** in `EntityTypeConfiguration` to enforce the *actual* database constraints (e.g., `.IsRequired()`, `.HasIndex().IsUnique()`).

> [!IMPORTANT]  
> While attributes define intent, the **Fluent API is the final authority**. If there is a conflict between an attribute and a Fluent API configuration, the database will follow the Fluent API.

### 2. Hardening via Fluent API
All "Hardening Rules" defined in the Core phase must be explicitly enforced in the `EntityTypeConfiguration`.

| Constraint Type | Implementation (Fluent API) | Purpose |
| :--- | :--- | :--- |
| **Mandatory Data** | `.Property(x => x.Name).IsRequired()` | Ensures non-nullable columns at the DB level. |
| **Uniqueness** | `.HasIndex(x => x.Slug).IsUnique()` | Prevents identity collisions (e.g., duplicate slugs). |
| **Value Constraints** | `.Property(x => x.Length).HasMaxLength(N)` | Enforces data length/size limits in the schema. |
| **Identity Integrity**| `.HasKey(x => x.Id)` | Defines the primary key for the entity. |

### 3. Global Query Filters (The Visibility Layer)
To ensure that `ISoftDeletable` and other visibility flags are respected across the entire application, we implement **Global Query Filters**.

*   **Mechanism**: The `AppDbContext` automatically applies a filter to every entity implementing `ISoftDeletable`.
*   **Rule**: Every query executed through the `DbContext` will implicitly include `WHERE IsDeleted = 0`, unless `.IgnoreQueryFilters()` is explicitly called.
*   **Purpose**: This provides a safety net, ensuring that "deleted" entities are invisible to the application by default.

### 4. Indexing Strategy
To maintain performance during identity resolution and scoped lookups, indexes must be applied strategically:

1.  **Identity/Routing Indexes**: Always place a `UNIQUE INDEX` on properties used for resolution (e.g., `Slug`).
2.  **Foreign Key Indexes**: All Foreign Keys (`UserId`, `ProjectId`, etc.) must have an index to optimize joins and hierarchical lookups.
3.  **Filtering Indexes**: Common filter criteria (e.g., `Status` or `TypeHint`) should be indexed to support high-performance discovery queries.

***

**How does this Pillar C draft look? Does it cover the technical enforcement details you expect, or is there another layer of integrity we need to document?**
==> guides/basics/developer-cheat-sheet.md <==
This is designed to be a high-density, single-page reference for developers. It strips away all theory and provides only the **logic gates**, **rules**, and **flows** required to write compliant code.

***

# ⚡ Anima Developer Cheat Sheet

## 🛠️ Decision Matrix: Structural Design

Use these matrices to determine how to implement a new class or component.

### 1. Autonomy Test (Module vs. Entity)
| Does the object need... | ...an independent Slug? | ...to be Searchable/Taggable? | ...its own Lifecycle? | **Result** |
| :--- | :---: | :---: | :---: | :--- |
| **YES to all** | ✅ Yes | ✅ Yes | ✅ Yes | **Entity** (`MetaEntityBase<T>`) |
| **NO to any** | ❌ No | ❌ No | ❌ No | **Module** (Plain Class) |

### 2. Scope Selection (If it is an Entity)
| If the object is... | Use this Soul Template | Inheritance Path |
| :--- | :--- | :--- |
| **Global/Root** (e.g., User, Project) | `BaseMetaInfo` | `BaseMetaInfo` $\rightarrow$ **Soul** |
| **Scoped** (e.g., Character, Task) | `ScopedMetaInfo` | `BaseMetaInfo` $\rightarrow$ `ScopedMetaInfo` $\rightarrow$ **Soul** |

---

## 🛡️ Data Integrity: The Hardening Rules

Apply these rules to every model definition to ensure structural stability and prevent identity drift.

| Property Type | Requirement | Implementation |
| :--- | :--- | :--- |
| **Identity/Scope Keys** (`Id`, `ScopeId`) | Must be immutable after creation. | Use `init` keyword. |
| **Core Attributes** (e.g., `Title`) | Must exist at instantiation. | Use `required` modifier. |
| **Data Constraints** (e.g., `Length`, `Range`) | Must be enforced at the model level. | Use Data Annotations (`[MaxLength]`, etc.). |

---

## 🔄 Runtime Logic: The Orchestration Loop

Every service method (the "Brain") must follow this three-step execution cycle to ensure security and reliability.

### **The Workflow**
1.  **Resolve**: Convert human-readable input (Slug) into the internal `Guid` identity using `SlugResolverService`.
2.  **Authorize**: Verify permissions via `CheckAccessAsync<T>` within the current scope.
3.  **Delegate**: Pass the validated request to a specialized **Component Service** (the "Muscle") for execution.

---

## 🌉 Identity Resolution: The Lookup Flow

When interacting with the API via slugs, the system follows this lookup path:

`Input String` $\rightarrow$ `SlugResolverService` $\rightarrow$ `Soul (BaseMetaInfo)` $\rightarrow$ `Identity (Guid)`

*   **Optimization**: If input is a valid GUID, the resolver bypasses the database lookup and returns the ID immediately.
*   **Scoped Lookup**: For scoped entities, the resolver ensures the `ScopeId` matches the provided context.

***

**Quick Rule of Thumb:**
- **Module** = Part of a Body (No Soul).
- **Entity** = Has a Soul (`MetaEntityBase`).
- **Soul** = The identity used for searching and routing (`BaseMetaInfo`).
==> guides/proof-of concept/command-based-services.md <==
You are absolutely right. Without explaining *why* we use certain patterns (like Model-First) and what specific risks those patterns introduce, the guide is incomplete. We need to document the trade-offs so that anyone working on the code understands the "defensive" logic built into it.

I will insert a dedicated section on **Security & Data Integrity** immediately after the Core Principles, explaining the Model-First approach and how we mitigate its inherent risks.

***

# 🏛️ Anima Architecture: The Blueprint

## 🧠 Core Principles & Patterns

### 1. The Body-Soul Pattern (Chunked Workflow)
To reduce cognitive load and prevent massive, complex requests, we split entity creation into two distinct steps. This allows the UI to provide immediate feedback after the "Soul" is created.

*   **Step 1: Create the Soul (Metadata)** $\rightarrow$ Returns `Id` + `Slug`.
*   **Step 2: Fill the Body (Content)** $\rightarrow$ Uses the `Slug` to update the entity properties.

### 2. Security & Data Integrity (The Model-First Trade-off)
We use a **Model-First** approach where our command payloads are the actual domain models themselves. This ensures a single source of truth but introduces two specific risks that we mitigate through strict patterns:

| Risk | Description | Our Counter-Action |
| :--- | :--- | :--- |
| **Payload Spoofing** | A client might try to "inject" properties (like `IsAdmin` or `Id`) into a command payload. | **Attribute Hardening**: We use `[ReadOnly]` and `[JsonIgnore]` on models. The `EntityMapper` is hard-coded to ignore these properties during mapping. |
| **Identity Desync** | If the Soul and Body are created separately, their IDs might drift apart. | **Law I (Object Initializer)**: We enforce that both must be instantiated with the same ID in a single atomic operation/transaction. |

### 3. Law I: Unified Identity
To keep the "Soul" and "Body" synchronized, they must share the same ID. This is achieved via **Object Initializers** during creation.
```csharp
// Ensuring Body.Id == Soul.Id at the moment of birth
var project = new Project { 
    Id = meta.Id, // Law I: Identity synchronization
    Description = "..." 
};
```

---

## 🔄 The Execution Flow
The Controller acts as the entry point, translating HTTP requests into Commands for the Bus to route.

`UI/Client` $\rightarrow$ `Controller` $\rightarrow$ `CommandBus` $\rightarrow$ `CommandHandler` (The Implementation)

---

## 🛠️ System Components

### 1. The Commands (`Anima.Core`)
Commands are immutable packets of intent. They contain only the data required for the action. **They do not carry permission requirements; those are handled by the execution layer.**

```csharp
/// <summary>
/// Represents the intent to create a project's metadata.
/// </summary>
public record CreateProjectMetaCommand(ProjectMetaInfo MetaInfo) : ICommand;

/// <summary>
/// Represents the intent to update an existing project's body properties via its slug.
/// </summary>
public record UpdateProjectBodyCommand(string ProjectSlug, Project BodyProperties) : ICommand;
```

### 2. The Bus (`Anima.Api/Infrastructure`)
The `ICommandBus` acts as a mediator, routing the command to the correct handler registered in the DI container.

### 3. The Handlers (`Anima.Api/Infrastructure` & `DomainHandlers`)
The `BaseCommandHandler<T, R>` provides the "Safety Net": automatic transaction management and error handling.

```csharp
public abstract class BaseCommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand
{
    protected readonly DbContext _db;
    protected readonly ICoreServicesProvider _core;

    // The Template Method: manages transaction and error handling
    public async Task<TResponse> HandleAsync(TCommand command)
    {
        using var tx = await _db.Database.BeginTransactionAsync();
        try {
            var response = await ExecuteAsync(command); // Actual work happens here

            if (response is ISuccessfulCommandResponse s && !s.IsSuccessful) await tx.RollbackAsync();
            else await tx.CommitAsync();

            return response;
        } catch (Exception ex) {
            await tx.RollbackAsync(); throw;
        }
    }

    protected abstract Task<TResponse> ExecuteAsync(TCommand command);
}
```

---

## 🚀 Usage Example: Project Lifecycle

### Step A: The Controller (The Entry Point)
```csharp
[HttpPost("projects/meta")]
public async Task<IActionResult> CreateMeta([FromBody] ProjectMetaInfo meta)
{
    // Dispatches the intent to the bus
    var response = await _commandBus.SendAsync(new CreateProjectMetaCommand(meta));
    return response.Successful ? Ok(response.Data) : BadRequest(response.Message);
}
```

### Step B: The Handler (The Implementation)
The handler performs the actual work, including permission checks using the `_core` provider.

```csharp
public class CreateProjectMetaCommandHandler : BaseCommandHandler<CreateProjectMetaCommand, CommandResponse<MetaEntityIdentityResponseDto>>
{
    protected override async Task<CommandResponse<MetaEntityIdentityResponseDto>> ExecuteAsync(CreateProjectMetaCommand command)
    {
        // 1. Authorization: Check permission using the core provider (NOT from payload)
        var auth = await _core.PermissionEngine.CheckAccessAsync(_userId, null, PermissionsEnum.CanCreate);
        if (auth.Status != AccessResultStatus.Allowed) 
            return CommandResponse<MetaEntityIdentityResponseDto>.Failure(auth.Message);

        // 2. Implementation: Create the Soul and return identity for Step 2
        var meta = await _db.MetaInfos.AddAsync(command.MetaInfo);
        await _db.SaveChangesAsync();

        return CommandResponse<MetaEntityIdentityResponseDto>.Success(new() { 
            Id = meta.Id, 
            Slug = meta.Slug 
        });
    }
}
```

---

## 📋 Summary of Development Rules

| Rule | Name | Instruction |
| :--- | :--- | :--- |
| **Rule 1** | **The Identity Law** | Never pass IDs in commands if a Slug is available. Always resolve identity within the handler to prevent client-side spoofing. |
| **Rule 2** | **No Payload Permissions** | Commands must never contain permission levels or roles. The handler must verify authorization using `ICoreServicesProvider`. |
| **Rule 3** | **The Single Responsibility** | One Command $\rightarrow$ One Handler. If a command is getting too large, split it into two distinct commands (e.g., Meta vs. Body). |
==> guides/system/authorization.md <==
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

==> guides/system/orchestration.md <==
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

==> guides/system/tagging.md <==
# Tagging System

The GaDeMa tagging system is built on a **Dual-Track Model**. Instead of treating all tags as identical database entries, the architecture distinguishes between structural, code-defined categories and flexible, user-driven organization.

This distinction ensures that core architectural constraints (like game mechanics or story structures) remain stable while allowing creators to apply infinite, organic categorization to their work.

---

## 1. Static Tags (Code-Defined)

Static tags are fundamental, structural elements of the system. They are defined as `Enums` in the codebase and represent categories that are intrinsic to the domain logic itself.

### Characteristics
* **Stability**: Since they are part of the compiled code, these tags are immutable at runtime.
* **Deterministic Identity**: Every enum member is mapped to a unique, predictable GUID via the `EnumGuidExtensions`. This allows them to be used as reliable identifiers in database relationships and complex logic.
* **Scope**: They represent "hard" categories such as `BaseTag` (e.g., Lore, Tutorial) or domain-specific tags like `GameTag` (e.g., Fire, Water) or `WritingTag`.

### Implementation Pattern
Static tags are defined using the `[ModuleIndex]` attribute to establish their position in the system's global indexing scheme.

```csharp
[ModuleIndex(2)]
public enum GameTag
{
    Fire = 0,
    Water = 1,
    Stunned = 2
}
```

---

## 2. Dynamic Tags (Database-Driven)

Dynamic tags are the "soft" organizational layer. They represent the organic, user-driven categorization that emerges during the creative process.

### Characteristics
* **Flexability**: Users can create, rename, and delete these tags at runtime without code changes.
* **Unstructured Growth**: They allow for infinite expansion (e.g., tagging a scene as "Spooky" or "High Tension") which cannot be predicted by developers.
* **Storage**: These are stored in the `MetaTag` table and linked to content items via many-to-many junction tables.

### Implementation Pattern
Dynamic tags are managed through the `IMetadataService`, which handles the creation and persistence of `MetaTag` entities within the database.

---

## 3. Comparison Summary

| Feature | Static Tags (Enums) | Dynamic Tags (Database) |
| :--- | :--- | :--- |
| **Definition** | Hardcoded in the codebase. | Created by users at runtime. |
| **Stability** | High (Immutable). | Low (Mutable). |
| **Identity** | Deterministic GUIDs via `[ModuleIndex]`. | Randomly generated UUIDs. |
| **Primary Use** | Structural classification & game mechanics. | Creative organization & discovery. |

## ⚖️ Trade-offs

| Approach | Pros | Cons |
| :--- | :--- | :--- |
| **Static Tags** | Extremely fast, type-safe, and provides a reliable "Source of Truth" for core logic. | Requires a code deployment to add or change categories. |
| **Dynamic Tags** | Provides infinite flexibility for the user; no technical friction for new ideas. | Harder to enforce strict rules; requires database management and can lead to fragmentation (e.g., "Spooky" vs "Spookey"). |

***
*Last Updated: [Date]*

==> guides/utils/EntityMapper.md <==
# 🛠️ EntityMapper Guide: The Secure Mapping Engine

The **`EntityMapper`** is the core component of GaDeMa’s "Model-First" architecture. It is not just a data transfer tool; it is a **security sieve** that ensures all data flowing from DTOs into Entities is sanitized, validated, and authorized according to the rules defined directly on your models.

By using this mapper, we move away from manual property assignments in services and toward a **declarative security model**.

---

## 🧬 Core Philosophy: The "Leaf-Only" Mapping

To prevent common reflection pitfalls—such as accidental navigation property replacement or unexpected database queries (N+1)—the `EntityMapper` is designed as a **Leaf-Only Mapper**.

*   **Scope**: It only maps "leaf" nodes: Primitives, Enums, Strings, and Structs.
*   **Safety**: It automatically ignores class-based properties (Navigation Properties). This ensures that you can never accidentally overwrite an entity's relationships via a mapping call, preserving the structural integrity of your database.

---

## 🛡️ Attribute-Driven Security & Integrity

The mapper respects four key attributes placed on your target entities. These allow you to define business rules directly in your models.

### 1. The Perimeter: `[MinRole]` (Class Level)
When applied to a **class**, this attribute acts as a "Perimeter Check." If the user's role does not meet the requirement, the entire mapping operation is aborted immediately.

| Attribute | Parameters | Description |
| :--- | :--- | :--- |
| `[MinRole]` | `MinimumRole`, `BypassRole?` | Defines a threshold (e.g., `Admin`) and an optional bypass role (e.g., `Owner`). |

**Example:**
```csharp
[MinRole(ProjectMemberRoleEnum.Editor, bypassRole: ProjectMemberRoleEnum.Owner)]
public class Story { ... } 
// Only Editors or Owners can trigger any mapping on this class.
```

### 2. The Precision: `[MinRole]` (Property Level)
When applied to a **property**, this provides "Granular Control." Even if the user passes the class-level check, they must still meet the requirement for specific sensitive fields.

**Example:**
```csharp
public class Story {
    public string Description { get; set; } // Anyone can edit description

    [MinRole(ProjectMemberRoleEnum.Admin, bypassRole: ProjectMemberRoleEnum.Owner)]
    public bool IsPublic { get; set; } // Only Admins or Owners can change this
}
```

### 3. Data Quality: `[Sanitize]` & `[Validate]`
These attributes ensure that the data being mapped is clean and valid.

| Attribute | Target Type | Action |
| :--- | :--- | :--- |
| `[Sanitize]` | `string` | Automatically strips HTML tags and trims whitespace. |
| `[Validate]` | Any | Triggers a validation check before the value is applied to the entity. |

### 4. Immutability: `[ReadOnly]`
*   **Action**: If a property is marked with `[ReadOnly]`, the mapper will skip it entirely, even if the source DTO contains a value.
*   **Use Case**: Protecting `Id`, `CreatedAt`, or other structural anchors that should never be modified after creation.

---

## ⚙️ Developer Workflow

### Step 1: Define your Model with Attributes
Decorate your entity with the rules you want to enforce.

```csharp
public class ProjectTask {
    [ReadOnly]
    public Guid Id { get; set; } // Cannot be overwritten

    [Sanitize]
    public string Title { get; set; } // HTML will be stripped

    [MinRole(ProjectMemberRoleEnum.Admin)]
    public bool IsPriority { get; set; } // Only Admins can change this
}
```

### Step 2: Use the Wrapper in your `DomainService`
Do not call the mapper directly from controllers. Always use the `MapProperties` wrapper provided by `DomainService`. This ensures that the user's role and scope are automatically resolved and passed to the engine.

```csharp
public async Task<ApiResponseDto<Story>> UpdateStoryAsync(Guid id, Story updateDto)
{
    var story = await _db.Stories.FindAsync(id);
    if (story == null) return ApiResponseDto<Story>.NotFound();

    // The wrapper automatically resolves the user's role and 
    // passes it to the EntityMapper internally.
    MapProperties(updateDto, story);

    await _db.SaveChangesAsync();
    return ApiResponseDto<Story>.Success(story);
}
```

---

## ⚖️ Summary Table for Developers

| If a property is marked... | And the User role is... | The Mapper will... |
| :--- | :--- | :--- |
| **`[ReadOnly]`** | Any Role | **Skip** the property. |
| **`[MinRole(Admin)]`** | `Editor` | **Skip** the property. |
| **`[MinRole(Admin, Bypass: Owner)]`** | `Owner` | **Apply** the value. |
| **`[Sanitize]`** | Any Role | **Clean** the string before applying. |
| **(No Attribute)** | Any Role | **Apply** the value (if it's a leaf type). |

***
*Last Updated: [Date]*
==> guides/utils/ServiceCollectionExtensions.md <==
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
==> guides/utils/string-sanitizer.md <==
# String Sanitization

The `StringSanitizer` utility provides a standardized way to transform human-readable input into machine-friendly, URL-safe strings. This is primarily used for generating "slugs" (e.g., converting `"My Awesome Story"` into `"my_awesome_story"`) which are used as identifiers in URLs and internal routing.

---

## 1. The `Normalize` Method

The core functionality of the utility is contained within the `Normalize` method. It follows a strict transformation pipeline to ensure consistent output.

### Transformation Pipeline

1.  **Case Normalization**: Converts the entire input string to lowercase invariant.
2.  **Regex Cleaning**: Uses a regular expression (`[^a-z0-9]+`) to identify any character that is not a lowercase letter or a digit.
3.  **Separator Replacement**: Replaces all identified special characters and whitespace with a single underscore (`_`).
4.  **Trimming**: Removes any leading or trailing underscores from the final result.

---

## 2. Usage Examples

The following table demonstrates how different inputs are processed by the `Normalize` method:

| Input String | Output (Slug) | Notes |
| :--- | :--- | :--- |
| `"Dark Tone"` | `dark_tone` | Space replaced by underscore |
| `"The Hero's Journey!"` | `the_hero_s_journey` | Special characters removed/replaced |
| `"  Space...  "` | `space` | Leading/trailing underscores trimmed |
| `"123-ABC_xyz"` | `123_abc_xyz` | Mixed alphanumeric and separators |

### Code Implementation Example

```csharp
// Basic usage in a controller or service
string userInput = "A New Adventure!";
string slug = StringSanitizer.Normalize(userInput);

// Result: "a_new_adventure"
```

---

## 3. Technical Constraints

* **Character Set**: The output is strictly limited to lowercase `a-z`, digits `0-9`, and the underscore `_`.
* **Empty Input**: If the input is null, empty, or consists only of whitespace, the method returns an empty string (`string.Empty`).
* **Lossy Transformation**: This process is "lossy"—once a string is normalized, you cannot perfectly reconstruct the original casing or special characters from the slug alone.

## ⚖️ Trade-offs

| Pros | Cons |
| :--- | :--- |
| **Consistency**: Ensures all URL slugs follow the same format across the entire system. | **Information Loss**: Cannot preserve case sensitivity, accented characters, or punctuation. |
| **Safety**: Produces strings that are safe for URLs and file systems, avoiding many injection/parsing issues. | **Single-Pass Logic**: The regex approach is efficient but simple; it does not handle complex linguistic rules (e.transliteration). |

***
*Last Updated: [Date]*
