namespace Anima.Core.Models.Shared;

/// <summary>
/// Defines the ownership models for polymorphic entities within a project.
/// </summary>
public enum OwnerTypeEnum
{
    /// <summary>The project is owned by a single individual user.</summary>
    [Display(Name = "User")]
    User = 0,

    /// <summary>The project is owned by a collaborative team.</summary>
    [Display(Name = "Team")]
    Team = 1,
}

