using System.Linq.Expressions;
using Anima.Core.Base.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Anima.Data.Contexts;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        ApplySoftDeleteFilters(modelBuilder);

        // This will automatically find and apply all IEntityTypeConfiguration 
        // implementations in the Anima.Data assembly (the ones we just moved).
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        SeedSystemUser(modelBuilder);
    }

     private void ApplySoftDeleteFilters(ModelBuilder modelBuilder)
    {
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(ISoftDeletable).IsAssignableFrom(entityType.ClrType))
            {
                // Build expression: e => e.IsDeleted == false
                var parameter = Expression.Parameter(entityType.ClrType, "e");
                var property = Expression.Property(parameter, nameof(ISoftDeletable.IsDeleted));
                var falseConstant = Expression.Constant(false);
                var equality = Expression.Equal(property, falseConstant);
                var lambda = Expression.Lambda(equality, parameter);

                modelBuilder.Entity(entityType.ClrType).HasQueryFilter(lambda);
            }
        }
    }
}
