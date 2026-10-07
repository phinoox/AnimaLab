using Anima.Core.Models.Projects;
using Anima.Core.Models.Tagging;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.Tagging;

public class ProjectTagRelationEntityTypeConfiguration : TagRelationEntityTypeConfiguration<ProjectTagRelation, ProjectMetaInfo>
{
    public override void Configure(EntityTypeBuilder<ProjectTagRelation> builder)
    {
        base.Configure(builder);
        // You can add project-specific index or property configuration here if needed
    }
}
