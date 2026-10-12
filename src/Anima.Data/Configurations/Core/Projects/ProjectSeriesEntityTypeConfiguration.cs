using Anima.Core.Models.Projects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Anima.Data.Configurations.Core.Projects;

public class ProjectSeriesEntityTypeConfiguration : IEntityTypeConfiguration<ProjectSeries>
{
    public void Configure(EntityTypeBuilder<ProjectSeries> builder)
    {
        builder.HasKey(e => e.Id);

    }
}
