using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Anima.Core.Models.ContentBase;

namespace Anima.Data.Configurations.Core.ContentBase;

/// <summary>
/// Configuration for AssetLink entity in engine integration system.
/// </summary>
public class AssetLinkEntityTypeConfiguration : IEntityTypeConfiguration<AssetLink>
{
    /// <summary>
    /// Configure AssetLink entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<AssetLink> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);
                
        // Navigation property: ContentMetaInfo (Cascade delete)
        builder.HasOne(al => al.MetaInfo)
            .WithMany(ci => ci.AssetLinks)
            .HasForeignKey(al => al.Id)
            .OnDelete(DeleteBehavior.Cascade);
        
        // Properties configuration
        builder.Property(e => e.EnginePath).IsRequired();
    }
}
