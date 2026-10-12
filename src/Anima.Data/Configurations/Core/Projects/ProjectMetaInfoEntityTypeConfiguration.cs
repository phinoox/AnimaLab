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

        // Map the relationship to UserMetaInfo instead of User
        builder.HasOne(m => m.UserProfile)
        .WithMany() // Or .WithOne() if you want a 1:1 link back
        .HasForeignKey(m => m.CreatedByUserId)
        .OnDelete(DeleteBehavior.Restrict);


        // Identity properties
        builder.Property(e => e.Status).IsRequired();
        builder.Property(e => e.ViewMode).IsRequired();

        // Indexes for performance and uniqueness
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.CreatedByUserId);
    }
}
