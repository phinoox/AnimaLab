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