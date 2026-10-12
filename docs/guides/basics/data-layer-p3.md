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