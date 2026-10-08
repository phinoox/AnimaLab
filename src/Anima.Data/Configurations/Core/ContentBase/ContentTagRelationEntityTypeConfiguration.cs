using Anima.Core.Models.ContentBase;
using Anima.Core.Models.Tagging;
using Anima.Data.Configurations.Core.Tagging;

namespace Anima.Data.Configurations.Core.ContentBase;

public class ContentTagRelationEntityTypeConfiguration : TagRelationEntityTypeConfiguration<ContentTagRelation, ContentMetaInfo>
{
    // No extra configuration needed unless content tags have unique rules
}