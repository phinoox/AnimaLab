using Anima.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.Projects;

/// <summary>
/// Configuration for ProjectMetaInfo entity, which acts as the identity anchor for a Project.
/// </summary>
public class ProjectMetaInfoEntityTypeConfiguration : IEntityTypeConfiguration<ProjectMetaInfo>
{
    public void Configure(EntityTypeBuilder<ProjectMetaInfo> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        // Identity properties
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.ViewMode).IsRequired();

        // Indexes for performance and uniqueness
        builder.HasIndex(e => e.Slug).IsUnique();
    }
}
