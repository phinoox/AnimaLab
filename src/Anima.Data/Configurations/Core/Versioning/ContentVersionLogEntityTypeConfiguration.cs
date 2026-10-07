// =============================================================================
using Anima.Core.Models.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
// Gadema.Core - Shared Domain Models & Interfaces
// =============================================================================

namespace Anima.Data.Configurations.Core.Versioning;

/// <summary>
/// Configuration for ContentVersionLog entity in version control system.
/// </summary>
public class ContentVersionLogEntityTypeConfiguration : IEntityTypeConfiguration<ContentVersionLog>
{
    public void Configure(EntityTypeBuilder<ContentVersionLog> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);

        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.MetaInfoId);
        builder.HasIndex(e => e.VersionNumber);  // Query recent versions
        
        // Navigation property: ContentMetaInfo (SetNull to preserve version history)
        builder.HasOne(cvl => cvl.ContentMetaInfo)
            .WithMany(ci => ci.VersionLogs)
            .HasForeignKey(cvl => cvl.MetaInfoId)
            .OnDelete(DeleteBehavior.SetNull);

        // Properties configuration
        builder.Property(e => e.ChangedByUserId).IsRequired();
        builder.Property(e => e.ChangeDescription).HasMaxLength(2048);
    }
}
