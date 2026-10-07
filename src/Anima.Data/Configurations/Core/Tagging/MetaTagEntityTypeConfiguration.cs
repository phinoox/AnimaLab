using Anima.Core.Models.Tagging;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.Tagging;

public class MetaTagEntityTypeConfiguration : IEntityTypeConfiguration<MetaTag>
{
    public void Configure(EntityTypeBuilder<MetaTag> builder)
    {
        builder.HasKey(t => t.Id);

        builder.HasIndex(t => t.Name).IsUnique();
        builder.HasIndex(t => t.Slug).IsUnique();
    }
}
