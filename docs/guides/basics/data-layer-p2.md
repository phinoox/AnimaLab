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
