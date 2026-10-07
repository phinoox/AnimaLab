namespace Anima.Core.Models.ContentBase;

/// <summary>
/// Links content items to game engine asset systems, facilitating tracking of exported assets.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class AssetLink
{
    /// <summary>
    /// Gets or sets the unique identifier for this asset link.
    /// </summary>
    public Guid Id { get; set; } = Guid.NewGuid();

    /// <summary>
    /// Gets or sets the unique identifier of the associated content item.
    /// </summary>
    [Required, Display(Name = "Content Item")]
    public Guid MetaInfoId { get; set; }

    /// <summary>
    /// Gets or sets the associated content meta information entity.
    /// </summary>
    [ForeignKey("MetaInfoId")]
    public virtual ContentMetaInfo ContentMetaInfo { get; set; }
    /// <summary>
    /// Gets or sets the path within the game engine's asset directory.
    /// </summary>
    [MaxLength(2048)]
    public string EnginePath { get; set; } = "";

    /// <summary>
    /// Gets or sets the optional unique identifier provided by the target game engine for cross-referencing.
    /// </summary>
    [MaxLength(128)]
    public string? EngineAssetId { get; set; }

    /// <summary>
    /// Gets or sets the file type used in the engine (e.g., "fbx", "uasset", "unitypackage").
    /// </summary>
    [MaxLength(64)]
    public string EngineFileType { get; set; } = "fbx";
}