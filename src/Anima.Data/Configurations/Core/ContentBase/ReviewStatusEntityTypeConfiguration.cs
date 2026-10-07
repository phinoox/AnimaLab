using Anima.Core.Models.ContentBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.ContentBase;

/// <summary>
/// Configuration for ReviewStatus entity in review workflow system.
/// </summary>
public class ReviewStatusEntityTypeConfiguration : IEntityTypeConfiguration<ReviewStatus>
{
    /// <summary>
    /// Configure ReviewStatus entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<ReviewStatus> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        // Note: TargetId is now unified with Id (Law I), so we index the primary identity.
        builder.HasIndex(e => e.Status);  // Filter by status
        builder.HasIndex(e => e.ReviewedByUserId).HasDatabaseName("IX_ReviewStatus_ReviewerId"); // Fixed property name
        builder.HasIndex(e => e.ReviewedAt);  // Query recent reviews
                
        // Navigation property: Reviewer (Optional FK to User)
        builder.HasOne(rs => rs.Reviewer)
            .WithMany()
            .HasForeignKey(rs => rs.ReviewedByUserId)
            .OnDelete(DeleteBehavior.Restrict);

        // Properties configuration
        builder.Property(e => e.Status).IsRequired();
    }
}
