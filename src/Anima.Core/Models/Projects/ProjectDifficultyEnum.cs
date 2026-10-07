namespace Anima.Core.Models.Projects;

/// <summary>
/// Defines the perceived complexity level of a project to assist in workload and resource management.
/// </summary>
public enum ProjectDifficultyEnum
{
    /// <summary>Low complexity; tasks are straightforward and require minimal coordination.</summary>
    [Display(Name = "Easy")]
    Easy = 0,

    /// <summary>Moderate complexity; requires planning, some research, or moderate coordination.</summary>
    [Display(Name = "Medium")]
    Medium = 1,

    /// <summary>High complexity; involves significant research, intricate design, or extensive implementation.</summary>
    [Display(Name = "Hard")]
    Hard = 2,
}

