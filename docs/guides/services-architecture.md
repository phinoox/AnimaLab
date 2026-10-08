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
