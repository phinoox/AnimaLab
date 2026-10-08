namespace Anima.Core.Dtos.Shared;


/// <summary>
/// Unified response for successful creation operations, containing navigation identifiers.
/// </summary>
public class CreateResponseDto
{
    /// <summary>
    /// The unique identifier of the newly created entity.
    /// </summary>
    [Required, Display(Name = "Entity ID")]
    public Guid EntityId { get; set; }
    
    /// <summary>
    /// The unique identifier of the associated meta-information record.
    /// </summary>
    [Required, Display(Name = "ContentMetaInfo ID")]
    public Guid MetaInfoId { get; set; }
    
    /// <summary>
    /// The unique identifier of the project this entity belongs to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
}


/// <summary>
/// Unified response for successful delete operations.
/// </summary>
public class DeleteResponseDto
{
    /// <summary>
    /// The unique identifier of the deleted entity.
    /// </summary>
    [Required, Display(Name = "Entity ID")]
    public Guid EntityId { get; set; }

    /// <summary>
    /// The unique identifier of the project the entity belonged to.
    /// </summary>
    [Required, Display(Name = "Project ID")]
    public Guid ProjectId { get; set; }
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

public class IdentityDto
{
   public Guid Id {get;set;}
   
}
