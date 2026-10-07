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

        // Relationship to ProjectSeries (1:1)
        // Law I: The Body's Id is its FK to the Soul. 
        // In this case, ProjectSeriesMetaInfo is the "Body" and ProjectSeries is the "Soul".
        builder.HasOne(mi => mi.ProjectSeries)
            .WithOne() // No navigation property back from Series in the model (Horizontal expansion uses ParentId/SeriesId usually, but here it's a 1:1 extension)
            .HasForeignKey<ProjectSeriesMetaInfo>(mi => mi.Id) // Updated: PK/FK Unification
            .OnDelete(DeleteBehavior.Cascade);
    }
}
