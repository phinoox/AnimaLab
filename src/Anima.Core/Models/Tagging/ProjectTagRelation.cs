using Anima.Core.Models.Projects;

namespace Anima.Core.Models.Tagging;


/// <summary>
/// Represents a junction between a project and its associated descriptive tags.
/// This enables categorization and discovery of entire projects through the tagging system.
/// </summary>
[ModelDependency(typeof(ProjectMetaInfo), typeof(MetaTag))] // Tells seeder: "Seed the MetaInfo first"
public class ProjectTagRelation : TagRelation<ProjectMetaInfo>
{
    // This class is now empty of logic, but it provides a concrete 
    // type for the DbContext and Seeder to target.
}