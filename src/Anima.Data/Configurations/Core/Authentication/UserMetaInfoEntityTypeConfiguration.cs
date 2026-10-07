using Anima.Core.Models.Authentication;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.Authentication;

/// <summary>
/// Configuration for UserMetaInfo entity.
/// </summary>
public class UserMetaInfoEntityTypeConfiguration : IEntityTypeConfiguration<UserMetaInfo>
{
    public void Configure(EntityTypeBuilder<UserMetaInfo> builder)
    {
        // The Id is shared with the User, so we don't use ValueGeneratedOnAdd here.
        builder.HasKey(e => e.Id);

        builder.Property(e => e.AvatarUrl).HasMaxLength(1024);
        builder.Property(e => e.ShortDesc).HasMaxLength(256);
        builder.Property(e => e.Title).HasMaxLength(128);
        builder.Property(e => e.Slug).HasMaxLength(128);
    }
}
