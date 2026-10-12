You are absolutely right. Without explaining *why* we use certain patterns (like Model-First) and what specific risks those patterns introduce, the guide is incomplete. We need to document the trade-offs so that anyone working on the code understands the "defensive" logic built into it.

I will insert a dedicated section on **Security & Data Integrity** immediately after the Core Principles, explaining the Model-First approach and how we mitigate its inherent risks.

***

# 🏛️ Anima Architecture: The Blueprint

## 🧠 Core Principles & Patterns

### 1. The Body-Soul Pattern (Chunked Workflow)
To reduce cognitive load and prevent massive, complex requests, we split entity creation into two distinct steps. This allows the UI to provide immediate feedback after the "Soul" is created.

*   **Step 1: Create the Soul (Metadata)** $\rightarrow$ Returns `Id` + `Slug`.
*   **Step 2: Fill the Body (Content)** $\rightarrow$ Uses the `Slug` to update the entity properties.

### 2. Security & Data Integrity (The Model-First Trade-off)
We use a **Model-First** approach where our command payloads are the actual domain models themselves. This ensures a single source of truth but introduces two specific risks that we mitigate through strict patterns:

| Risk | Description | Our Counter-Action |
| :--- | :--- | :--- |
| **Payload Spoofing** | A client might try to "inject" properties (like `IsAdmin` or `Id`) into a command payload. | **Attribute Hardening**: We use `[ReadOnly]` and `[JsonIgnore]` on models. The `EntityMapper` is hard-coded to ignore these properties during mapping. |
| **Identity Desync** | If the Soul and Body are created separately, their IDs might drift apart. | **Law I (Object Initializer)**: We enforce that both must be instantiated with the same ID in a single atomic operation/transaction. |

### 3. Law I: Unified Identity
To keep the "Soul" and "Body" synchronized, they must share the same ID. This is achieved via **Object Initializers** during creation.
```csharp
// Ensuring Body.Id == Soul.Id at the moment of birth
var project = new Project { 
    Id = meta.Id, // Law I: Identity synchronization
    Description = "..." 
};
```

---

## 🔄 The Execution Flow
The Controller acts as the entry point, translating HTTP requests into Commands for the Bus to route.

`UI/Client` $\rightarrow$ `Controller` $\rightarrow$ `CommandBus` $\rightarrow$ `CommandHandler` (The Implementation)

---

## 🛠️ System Components

### 1. The Commands (`Anima.Core`)
Commands are immutable packets of intent. They contain only the data required for the action. **They do not carry permission requirements; those are handled by the execution layer.**

```csharp
/// <summary>
/// Represents the intent to create a project's metadata.
/// </summary>
public record CreateProjectMetaCommand(ProjectMetaInfo MetaInfo) : ICommand;

/// <summary>
/// Represents the intent to update an existing project's body properties via its slug.
/// </summary>
public record UpdateProjectBodyCommand(string ProjectSlug, Project BodyProperties) : ICommand;
```

### 2. The Bus (`Anima.Api/Infrastructure`)
The `ICommandBus` acts as a mediator, routing the command to the correct handler registered in the DI container.

### 3. The Handlers (`Anima.Api/Infrastructure` & `DomainHandlers`)
The `BaseCommandHandler<T, R>` provides the "Safety Net": automatic transaction management and error handling.

```csharp
public abstract class BaseCommandHandler<TCommand, TResponse> : ICommandHandler<TCommand, TResponse>
    where TCommand : ICommand
{
    protected readonly DbContext _db;
    protected readonly ICoreServicesProvider _core;

    // The Template Method: manages transaction and error handling
    public async Task<TResponse> HandleAsync(TCommand command)
    {
        using var tx = await _db.Database.BeginTransactionAsync();
        try {
            var response = await ExecuteAsync(command); // Actual work happens here

            if (response is ISuccessfulCommandResponse s && !s.IsSuccessful) await tx.RollbackAsync();
            else await tx.CommitAsync();

            return response;
        } catch (Exception ex) {
            await tx.RollbackAsync(); throw;
        }
    }

    protected abstract Task<TResponse> ExecuteAsync(TCommand command);
}
```

---

## 🚀 Usage Example: Project Lifecycle

### Step A: The Controller (The Entry Point)
```csharp
[HttpPost("projects/meta")]
public async Task<IActionResult> CreateMeta([FromBody] ProjectMetaInfo meta)
{
    // Dispatches the intent to the bus
    var response = await _commandBus.SendAsync(new CreateProjectMetaCommand(meta));
    return response.Successful ? Ok(response.Data) : BadRequest(response.Message);
}
```

### Step B: The Handler (The Implementation)
The handler performs the actual work, including permission checks using the `_core` provider.

```csharp
public class CreateProjectMetaCommandHandler : BaseCommandHandler<CreateProjectMetaCommand, CommandResponse<MetaEntityIdentityResponseDto>>
{
    protected override async Task<CommandResponse<MetaEntityIdentityResponseDto>> ExecuteAsync(CreateProjectMetaCommand command)
    {
        // 1. Authorization: Check permission using the core provider (NOT from payload)
        var auth = await _core.PermissionEngine.CheckAccessAsync(_userId, null, PermissionsEnum.CanCreate);
        if (auth.Status != AccessResultStatus.Allowed) 
            return CommandResponse<MetaEntityIdentityResponseDto>.Failure(auth.Message);

        // 2. Implementation: Create the Soul and return identity for Step 2
        var meta = await _db.MetaInfos.AddAsync(command.MetaInfo);
        await _db.SaveChangesAsync();

        return CommandResponse<MetaEntityIdentityResponseDto>.Success(new() { 
            Id = meta.Id, 
            Slug = meta.Slug 
        });
    }
}
```

---

## 📋 Summary of Development Rules

| Rule | Name | Instruction |
| :--- | :--- | :--- |
| **Rule 1** | **The Identity Law** | Never pass IDs in commands if a Slug is available. Always resolve identity within the handler to prevent client-side spoofing. |
| **Rule 2** | **No Payload Permissions** | Commands must never contain permission levels or roles. The handler must verify authorization using `ICoreServicesProvider`. |
| **Rule 3** | **The Single Responsibility** | One Command $\rightarrow$ One Handler. If a command is getting too large, split it into two distinct commands (e.g., Meta vs. Body). |