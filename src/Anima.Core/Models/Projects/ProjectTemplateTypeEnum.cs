namespace Anima.Core.Models.Projects;

/// <summary>
/// Defines the available archetypes for project templates to provide standardized starting structures.
/// </summary>
public enum ProjectTemplateTypeEnum
{
    /// <summary>
    /// A template optimized for traditional fantasy prose and storytelling.
    /// </summary>
    [Display(Name = "Fantasy Book")]
    FantasyBook = 0,

    /// <summary>
    /// A template focused on game mechanics, combat, and RPG-style progression.
    /// </summary>
    [Display(Name = "Action RPG")]
    ActionRPG = 1,

    /// <summary>
    /// A template centered around science fiction themes and technology.
    /// </summary>
    [Display(Name = "Sci-Fi")]
    SciFi = 2,

    /// <summary>
    /// A template designed for suspenseful or frightening narratives.
    /// </summary>
    [Display(Name = "Horror/Thriller")]
    HorrorThriller = 3,

    /// <summary>
    /// A template focused on interpersonal relationships and emotional arcs.
    /// </summary>
    [Display(Name = "Romance")]
    Romance = 4,

    /// <summary>
    /// A template centered around investigation and suspenseful discovery.
    /// </summary>
    [Display(Name = "Mystery")]
    Mystery = 5,

    /// <summary>
    /// A dedicated horror-focused narrative structure.
    /// </summary>
    [Display(Name = "Horror")]
    Horror = 6,
}

