I have created a comprehensive guide for the new Search implementation. You can save this as `docs/guides/architecture/search-system.md`.

***

# 🔍 Search System Guide: Unified & Extensible Architecture

## 🌟 Overview
The Anima API implements a **Unified Search System**. Instead of having separate search endpoints for every entity (Projects, Blogs, Characters, etc.), we use an orchestration pattern that allows a single endpoint to query across the entire domain.

This system is designed to be **"Plug-and-Play"**: adding a new searchable entity requires zero changes to the Controller or the Orchestrator.

---

## 🏗️ Architecture Overview

The search system consists of four primary components:

1.  **`ISearchableProvider` (Interface)**: The contract that any domain entity must implement to become "searchable."
2.  **`SearchOrchestrator` (Engine)**: Aggregates results from all registered providers, handles parallel execution, type-based filtering, and pagination.
3.  **`SearchController` (Gateway)**: A single entry point for all search requests via `GET /api/search`.
4.  **Reflection-based Registration**: An extension method that automatically discovers and registers new providers at startup.

### 🔄 The Request Flow
1.  **HTTP Request** arrives at `SearchController`.
2.  The **`SearchRequestDto`** captures the query, optional type filter, pagination, and project context.
3.  The **`SearchOrchestrator`** receives the request:
    *   It identifies which providers match the requested `type` (or uses all if no type is specified).
    *   It triggers `GetMatchesAsync` on all relevant providers in **parallel** using `Task.WhenAll`.
4.  The results are aggregated, paginated, and returned to the client.

---

## 🛠️ Key Components

### 1. `ISearchableProvider`
Every provider must implement this interface. It requires a `SearchType` property which acts as the unique identifier for that domain (e.g., `"Project"`, `"BlogPost"`).

```csharp
public interface ISearchableProvider
{
    // The unique name of the resource type (used for filtering)
    string SearchType { get; }

    // The logic to query the database and map to a SearchHitDto
    Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId);
}
```

### 2. `SearchOrchestrator`
The heart of the system. It is responsible for:
*   **Parallelism**: Running all provider tasks concurrently to minimize latency.
*   **Filtering**: If a user requests `type=Project`, it only invokes providers where `SearchType == "Project"`.
*   **Pagination**: Applying `.Skip()` and `.Take()` logic to the aggregated results.

### 3. Automated Registration (`AddSearchProviders`)
We use reflection to eliminate boilerplate. In `Program.cs`, we call:
`builder.Services.AddSearchProviders(typeof(Program).Assembly);`

This scans the assembly for any class implementing `ISearchableProvider` and registers it automatically.

---

## 🚀 How to Extend (Adding a New Searchable Entity)

To make a new entity (e.g., `Character`) searchable, follow these three steps:

### Step 1: Create the Provider
Create a class in `Infrastructure/Providers/` that implements `ISearchableProvider`.

```csharp
public class CharacterSearchProvider : ISearchableProvider
{
    private readonly MyDbContext _db;
    public CharacterSearchProvider(MyDbContext db) => _db = db;

    // 1. Define the Type
    public string SearchType => "Character";

    // 2. Implement the Query Logic
    public async Task<IEnumerable<SearchHitDto>> GetMatchesAsync(string query, Guid? projectId)
    {
        return await _db.Characters
            .Where(c => c.Name.Contains(query)) // Your custom logic
            .Select(c => new SearchHitDto {
                ResourceId = c.Id,
                DisplayName = c.Name,
                Slug = c.Slug,
                ResourceType = "Character",
                ResourceLink = $"/api/v1/characters/{c.Slug}"
            })
            .ToListAsync();
    }
}
```

### Step 2: No Manual Registration Required!
Because of the `AddSearchProviders` extension method in `Program.cs`, your new provider is **automatically discovered** and injected into the Orchestrator at startup.

### Step 3: Test via API
Your new entity is immediately available via the unified endpoint:
*   **Global**: `GET /api/search?query=name`
*   **Specific**: `GET /api/search?query=name&type=Character`

---

## 📖 API Reference

### **Endpoint: `GET /api/search`**

| Parameter | Type | Required | Description |
| :--- | :--- | :--- | :--- |
| `query` | `string` | **Yes** | The search term. |
| `type` | `string` | No | Filters results to a specific `SearchType`. |
| `projectId` | `Guid` | No | Limits search to entities belonging to this project. |
| `page` | `int` | No | Page number (default: 1). |
| `pageSize` | `int` | No | Results per page (default: 10). |

**Response Example:**
```json
{
  "items": [
    {
      "resourceId": "...",
      "displayName": "My Blog Post",
      "slug": "my-blog-post",
      "resourceType": "BlogPost",
      "scopeId": "...",
      "resourceLink": "/api/v1/blog-posts/my-blog-post"
    }
  ],
  "totalCount": 1
}
```