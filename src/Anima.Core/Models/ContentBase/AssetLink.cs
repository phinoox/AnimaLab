namespace Anima.Core.Models.ContentBase;

/// <summary>
/// Links content items to game engine asset systems, facilitating tracking of exported assets.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo))]
public class AssetLink : MetaEntity<ContentMetaInfo>
{
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