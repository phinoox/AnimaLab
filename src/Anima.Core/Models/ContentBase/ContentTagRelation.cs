using Anima.Core.Models.Tagging;

namespace Anima.Core.Models.ContentBase;

/// <summary>
/// Represents a junction between a piece of content and its associated descriptive tags.
/// This enables categorization and discovery through the tagging system.
/// </summary>
[ModelDependency(typeof(ContentMetaInfo), typeof(MetaTag))] // Tells seeder: "Seed the MetaInfo first"
public class ContentTagRelation : TagRelation<ContentMetaInfo>
{
}