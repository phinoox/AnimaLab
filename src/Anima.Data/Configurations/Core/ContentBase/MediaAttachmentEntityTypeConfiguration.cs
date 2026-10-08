using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Anima.Core.Models.ContentBase;

namespace Anima.Data.Configurations.Core.ContentBase;

/// <summary>
/// Configuration for MediaAttachment entity in game development management system.
/// </summary>
public class MediaAttachmentEntityTypeConfiguration : IEntityTypeConfiguration<MediaAttachment>
{
    /// <summary>
    /// Configure MediaAttachment entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<MediaAttachment> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Id);
        
        // Navigation property: ContentMetaInfo (SetNull to preserve attachment history)
        builder.HasOne(m => m.MetaInfo)
            .WithMany(ci => ci.MediaAttachments)
            .HasForeignKey(m => m.Id)
            .OnDelete(DeleteBehavior.SetNull);  // Keep attachments when content deleted
        
        // Properties configuration
        builder.Property(e => e.FileName).IsRequired();
    }
}
