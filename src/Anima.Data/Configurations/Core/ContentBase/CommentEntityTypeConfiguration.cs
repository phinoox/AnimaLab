using Anima.Core.Models.ContentBase;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.ContentBase;

/// <summary>
/// Configuration for Comment entity in game development management system.
/// </summary>
public class CommentEntityTypeConfiguration : IEntityTypeConfiguration<Comment>
{
    /// <summary>
    /// Configure Comment entity properties and relationships.
    /// </summary>
    public void Configure(EntityTypeBuilder<Comment> builder)
    {
        // Primary key (The Soul ID)
        builder.HasKey(e => e.Id);
        
        // Indexes for frequently filtered columns
        // Note: TargetId has been unified with Id under Law I, so we index the primary identity.
        builder.HasIndex(e => e.AuthorUserId).HasDatabaseName("IX_Comment_AuthorUserId"); 
        builder.HasIndex(e => e.CreatedAt);
        builder.HasIndex(e => e.ParentCommentId); 
        
        // Properties configuration
        builder.Property(e => e.Text).IsRequired().HasMaxLength(4096); 
        builder.Property(e => e.CreatedAt).IsRequired();
    }
}
