namespace Anima.Core.Dtos.Shared;

public class IdentityDto
{
    public Guid Id{get;set;}
}

/// <summary>
/// Response data for an automatic save operation.
/// </summary>
public class AutosaveResponseDto
{
    /// <summary>
    /// The unique identifier of the created version log entry.
    /// </summary>
    public Guid VersionLogId { get; set; }

    /// <summary>
    /// The resulting version number after the save operation.
    /// </summary>
    public int VersionNumber { get; set; }
}

/// <summary>
/// ResponseDto for entities that follow the body-soul pattern with unified id
/// </summary>
public class MetaIdentityResponseDto
{
   public Guid Id {get;set;}

   public string Slug {get;set;}
   
}


/// <summary>
/// ResponseDto for common,mostly leaf, entities that don't follow the body-sould pattern
/// </summary>
public class ScopedIdentityResponseDto
{
    /// <summary>
    /// The unique identifier of the newly created entity.
    /// </summary>
    [Required, Display(Name = "Entity ID")]
    public Guid EntityId { get; set; }
    
    /// <summary>
    /// The unique identifier of the project this entity belongs to.
    /// </summary>
    [Required, Display(Name = "Parent ID")]
    public Guid ParentId { get; set; }
}

/// <summary>
/// General dto for commands that only singal if they were successfull
/// </summary>
public class SuccessResponseDto
{
    public bool Success {get;set;}
}