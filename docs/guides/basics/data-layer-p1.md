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
