using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Anima.Core.Models.ContentBase;

namespace Anima.Data.Configurations.Core.ContentBase;

/// <summary>
/// Configuration for ExternalReference entity in game development management system.
/// </summary>
public class ExternalReferenceEntityTypeConfiguration : IEntityTypeConfiguration<ExternalReference>
{
    /// <summary>
    /// Configure ExternalReference entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ExternalReference> builder)
    {
        // Primary key
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        builder.HasIndex(e => e.Url).IsUnique();
        builder.HasIndex(e => e.Title); // Added index for search performance

        // Relationship: Link to the ContentMetaInfo anchor (The "Soul")
        builder.HasOne(er => er.MetaInfo)
            .WithMany() 
            .HasForeignKey(e => e.Id)
            .OnDelete(DeleteBehavior.Cascade); // If MetaInfo is deleted, reference is gone
        
        // Properties configuration
        builder.Property(e => e.Url).IsRequired().HasMaxLength(2048);
        builder.Property(e => e.Title).HasMaxLength(128);
        builder.Property(e => e.Author).HasMaxLength(128);
        builder.Property(e => e.Notes).HasMaxLength(4096);
    }
}
