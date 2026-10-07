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

        // Relationship to Project (1:1)
        // Law I: The Body's Id is its FK to the Soul. 
        // In this case, ProjectMetaInfo is the "Body" and Project is the "Soul".
        builder.HasOne(mi => mi.Project)
            .WithOne(p => p.ProjectMetaInfo)
            .HasForeignKey<ProjectMetaInfo>(mi => mi.Id) // Updated: PK/FK Unification
            .OnDelete(DeleteBehavior.Cascade);

        // Indexes for performance and uniqueness
        builder.HasIndex(e => e.ProjectId).IsUnique();
    }
}
