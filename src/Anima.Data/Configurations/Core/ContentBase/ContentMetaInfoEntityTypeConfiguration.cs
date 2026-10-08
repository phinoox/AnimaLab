using Anima.Core.Models.ContentBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.ContentBase;

/// <summary>
/// Configuration for ContentMetaInfo entity in game development management system.
/// </summary>
public class MetaInfoEntityTypeConfiguration : IEntityTypeConfiguration<ContentMetaInfo>
{
    /// <summary>
    /// Configure ContentMetaInfo entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ContentMetaInfo> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Slug).IsUnique();
        builder.HasIndex(e => e.Status);
        builder.HasIndex(e => e.ProjectId);
        
        // Navigation property: MediaAttachments (SetNull to preserve attachment history)
        builder.HasMany(ci => ci.MediaAttachments)
            .WithOne(m => m.MetaInfo)
            .HasForeignKey(m => m.Id)
            .OnDelete(DeleteBehavior.SetNull);
        
        builder.HasOne(mi => mi.Project)
            .WithMany()
            .HasForeignKey(mi => mi.ProjectId)
            .OnDelete(DeleteBehavior.Cascade); 
        
        // Navigation property: MetaInfoTags (SetNull to preserve tags)
        builder.HasMany(ci => ci.MetaInfoTagRelations)
            .WithOne(ct => ct.MetaInfo)
            .HasForeignKey(ct => ct.MetaInfoId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}
