namespace Anima.Core.Models.Tagging;

/// <summary>
/// Defines fundamental tags available for all entities within the GaDeMa system.
/// </summary>
[ModuleIndex(0)]
public enum BaseTag : int
{
    /// <summary>Indicates a tutorial or instructional content.</summary>
    Tutorial, // Serial 0

    /// <summary>Indicates lore-related content for world-building.</summary>
    Lore,     // Serial 1

    /// <summary>Indicates reference material for further study or documentation.</summary>
    Reference // Serial 2
}
