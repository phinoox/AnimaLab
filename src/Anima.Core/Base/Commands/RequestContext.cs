namespace Anima.Core.Base.Commands;

public record RequestContext(
    Guid? ProjectId = null, 
    Guid? EntityId = null,
    Guid? ParentId = null
);