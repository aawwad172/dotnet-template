using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Interfaces.Application.Services;
using Dotnet.Template.Infrastructure.Configurations;
using Dotnet.Template.Infrastructure.Configurations.Seed;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Dotnet.Template.Infrastructure.Persistence;

public class BaseDbContext(DbContextOptions options, IServiceProvider serviceProvider, IConfiguration configuration) : DbContext(options)
{
    protected IServiceProvider _serviceProvider { get; } = serviceProvider;
    private readonly IConfiguration _configuration = configuration;
    // DbSet properties for the main entities and join tables
    public DbSet<User> Users { get; set; }
    public DbSet<Role> Roles { get; set; }
    public DbSet<Permission> Permissions { get; set; }
    public DbSet<UserRole> UserRoles { get; set; }
    public DbSet<RolePermission> RolePermissions { get; set; }
    public DbSet<UserPermissionOverride> UserPermissionOverrides { get; set; }

    public DbSet<RefreshToken> RefreshTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        // Apply configurations in specific order
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new UsersSeed(_configuration));

        modelBuilder.ApplyConfiguration(new RoleConfiguration());
        modelBuilder.ApplyConfiguration(new RolesSeed());

        modelBuilder.ApplyConfiguration(new PermissionConfiguration());
        modelBuilder.ApplyConfiguration(new PermissionsSeed());

        // Apply relationship configurations last
        modelBuilder.ApplyConfiguration(new UserRoleConfiguration());
        modelBuilder.ApplyConfiguration(new UsersRolesSeed());

        modelBuilder.ApplyConfiguration(new RolePermissionConfiguration());
        modelBuilder.ApplyConfiguration(new RolesPermissionsSeed());

        modelBuilder.ApplyConfiguration(new UserPermissionOverrideConfiguration());

        modelBuilder.ApplyConfiguration(new RefreshTokenConfiguration());

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
