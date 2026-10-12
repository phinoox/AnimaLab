using Anima.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.Projects;

public class ProjectEntityTypeConfiguration : IEntityTypeConfiguration<Project>
{
    public void Configure(EntityTypeBuilder<Project> builder)
    {
        builder.HasKey(e => e.Id);

        // --- Identity is now handled by ProjectMetaInfo ---
        // We no longer configure Title, Slug, Status, or Visibility here.

        // --- Domain Indexes ---
        builder.HasIndex(e => e.ProjectSeriesId);

        // --- Relationships ---
        // Define the 1:1 relationship with MetaInfo using a shared Primary Key
        builder.HasOne(p => p.MetaInfo) // Assumes MetaEntityBase defines this property
        .WithOne()
        .HasForeignKey<ProjectMetaInfo>(m => m.Id);

       
        // ProjectSeries
        builder.HasOne(p => p.ProjectSeries)
            .WithMany(ps => ps.Projects)
            .HasForeignKey(p => p.ProjectSeriesId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
