using Anima.Core.Models.Authentication;
using Anima.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;

namespace Anima.Data.Contexts;

public partial class AppDbContext
{
    private void SeedSystemUser(ModelBuilder modelBuilder)
    {
       var systemId = UserConstants.SystemUserId;
    // Define a static, fixed date for all seeded entities
    var seedDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc);

      // 2. Seed the UserMetaInfo (The Identity Anchor)
    modelBuilder.Entity<UserMetaInfo>()
        .HasData(new UserMetaInfo
        {
            Id = systemId,
            CreatedByUserId = systemId,
            AvatarUrl = "/images/system-avatar.png",
            // EXPLICITLY set these to static values as well:
            CreatedAt = seedDate,
            LastModifiedAt = seedDate
        });

    // 1. Seed the User account
    modelBuilder.Entity<User>()
        .HasData(new User
        {
            Id = systemId,
            UserName = "System",
            Email = "system@anima.com",
            IsActive = true,
            Provider = UserAuthProviderEnum.Password,
        });

  
    }

}