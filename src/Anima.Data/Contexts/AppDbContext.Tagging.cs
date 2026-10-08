using Anima.Core.Models.Tagging;
using Microsoft.EntityFrameworkCore;

namespace Anima.Data.Contexts;

public partial class AppDbContext
{
    public DbSet<MetaTag> MetaTags { get; set; }
    public DbSet<ProjectTagRelation> ProjectTagRelations { get; set; }
}
