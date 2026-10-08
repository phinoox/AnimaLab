using Anima.Core.Models.Shared;
using Anima.Core.Models.Versioning;
using Microsoft.EntityFrameworkCore;

namespace Anima.Data.Contexts;

public partial class AppDbContext
{
    public DbSet<ContentSnapshot> ContentSnapshots { get; set; }
    public DbSet<ContentVersionLog> ContentVersionLogs { get; set; }

    public DbSet<ActivityLog> ActivityLogs {get;set;}
}
