using Anima.Core.Models.Shared;
using Anima.Core.Models.Tagging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.Tagging;

/// <summary>
/// Base configuration for junction tables between MetaInfo and Tags.
/// </summary>
public abstract class TagRelationEntityTypeConfiguration<T, TMetaInfo> : IEntityTypeConfiguration<T> 
    where T : TagRelation<TMetaInfo>
    where TMetaInfo : BaseMetaInfo
{
    public virtual void Configure(EntityTypeBuilder<T> builder)
    {
        // Composite Primary Key: A piece of meta-info can only have a specific tag once
        builder.HasKey(r => new { r.MetaInfoId, r.TagId });

        // Relationship to the MetaInfo anchor
        builder.HasOne(r => r.MetaInfo)
            .WithMany() 
            .HasForeignKey(r => r.MetaInfoId)
            .OnDelete(DeleteBehavior.Cascade);

        // Relationship to the Tag
        builder.HasOne(r => r.Tag)
            .WithMany() 
            .HasForeignKey(r => r.TagId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
