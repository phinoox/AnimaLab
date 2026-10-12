
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

