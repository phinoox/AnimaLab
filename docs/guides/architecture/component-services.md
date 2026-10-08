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
