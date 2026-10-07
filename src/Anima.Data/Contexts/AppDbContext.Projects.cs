using Anima.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;

namespace Anima.Data.Contexts;

public partial class AppDbContext
{
    public DbSet<Project> Projects { get; set; }
    public DbSet<ProjectSeries> ProjectSeries { get; set; }
    public DbSet<ProjectMetaInfo> ProjectMetaInfos { get; set; }
    public DbSet<ProjectSeriesMetaInfo> ProjectSeriesMetaInfos { get; set; }
}
