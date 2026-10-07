using Anima.Core.Models.Authentication;
using Microsoft.EntityFrameworkCore;

namespace Anima.Data.Contexts;

public partial class AppDbContext
{
    public DbSet<User> Users { get; set; }
    public DbSet<ProjectMember> ProjectMembers { get; set; }
    public DbSet<UserMetaInfo> UserMetaInfos { get; set; }
}
