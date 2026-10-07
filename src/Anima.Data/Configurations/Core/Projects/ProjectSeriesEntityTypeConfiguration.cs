using Anima.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.Projects;

public class ProjectSeriesEntityTypeConfiguration : IEntityTypeConfiguration<ProjectSeries>
{
    public void Configure(EntityTypeBuilder<ProjectSeries> builder)
    {
        builder.HasKey(e => e.Id);

        // Note: Title, Slug, and Description are no longer on the ProjectSeries entity itself.
        // They are now managed by ProjectSeriesMetaInfo.

        // Relationships
        builder.HasOne(e => e.ProjectSeriesMetaInfo)
               .WithOne() 
               .OnDelete(DeleteBehavior.Cascade);

        // The inverse relationship (Projects -> ProjectSeries) is configured in ProjectEntityTypeConfiguration
    }
}
