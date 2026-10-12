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