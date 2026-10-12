using Anima.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.Projects;

/// <summary>
/// Configuration for ProjectSeriesMetaInfo entity, which acts as the identity anchor for a Project Series.
/// </summary>
public class ProjectSeriesMetaInfoEntityTypeConfiguration : IEntityTypeConfiguration<ProjectSeriesMetaInfo>
{
    public void Configure(EntityTypeBuilder<ProjectSeriesMetaInfo> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);

        // Indexes (Inherited properties from BaseMetaInfo)
        builder.HasIndex(e => e.Slug).IsUnique();

        // Define the relationship from the Soul (Dependent) to the Body (Principal)
    builder.HasOne(mi => mi.ProjectSeries) // This links back to the navigation property in MetaInfo
        .WithOne(ps => ps.MetaInfo)      // This tells EF that ProjectSeries has a 'MetaInfo' property
        .HasForeignKey<ProjectSeriesMetaInfo>(mi => mi.Id) // The ID is the FK
        .OnDelete(DeleteBehavior.Cascade);
    }
}
