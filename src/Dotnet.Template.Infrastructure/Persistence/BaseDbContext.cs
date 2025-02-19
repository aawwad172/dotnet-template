using Dotnet.Template.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dotnet.Template.Infrastructure.Persistence;
public class BaseDbContext(DbContextOptions options, IServiceProvider serviceProvider) : DbContext(options)
{
    protected IServiceProvider _serviceProvider { get; } = serviceProvider;

    public DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(BaseDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }

    // Logger service
    private ILogger<BaseDbContext> Logger => _serviceProvider.GetRequiredService<ILogger<BaseDbContext>>();

    public override async Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
    {
        // Log database changes before saving
        LogChanges();

        int result = await base.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);

        // Log successful save
        Logger.LogInformation("Database changes successfully saved. Affected rows: {Result}", result);

        return result;
    }

    private void LogChanges()
    {
        foreach (EntityEntry entry in ChangeTracker.Entries())
        {
            if (entry.State == EntityState.Added)
            {
                Logger.LogInformation($"Adding new entity: {entry.Entity.GetType().Name} - Values: {entry.CurrentValues.ToObject()}");
            }
            else if (entry.State == EntityState.Modified)
            {
                Logger.LogInformation($"Updating entity: {entry.Entity.GetType().Name} - Old Values: {entry.OriginalValues.ToObject()} - New Values: {entry.CurrentValues.ToObject()}");
            }
            else if (entry.State == EntityState.Deleted)
            {
                Logger.LogInformation($"Deleting entity: {entry.Entity.GetType().Name} - Values: {entry.OriginalValues.ToObject()}");
            }
        }
    }
}
