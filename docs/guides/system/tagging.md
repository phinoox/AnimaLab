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
