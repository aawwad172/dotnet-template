using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

using Microsoft.EntityFrameworkCore;

namespace Dotnet.Template.Infrastructure.Persistence.Repositories;

public class AuthenticationRepository(BaseDbContext dbContext) : IAuthenticationRepository
{
    private readonly BaseDbContext _dbContext = dbContext;

    public async Task AddUserRoleAsync(UserRole userRole)
    {
        await _dbContext.UserRoles.AddAsync(userRole);
    }

    public async Task<List<string>> GetAllPermissionNamesAsync()
    {
        return await _dbContext.Permissions
               .Select(p => p.Name)
               .ToListAsync();
    }

    public async Task<List<string>> GetBaseGrantedPermissionsAsync(List<Guid> roleIds)
    {
        // Query RolePermissions for all permissions granted by the user's roles
        return await _dbContext.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId))
            .Select(rp => rp.Permission.Name)
            .Distinct()
            .ToListAsync();
    }

    public async Task<List<Guid>> GetUserRoleIdsAsync(Guid userId)
    {
        return await _dbContext.UserRoles
               .Where(ur => ur.UserId == userId)
               .Select(ur => ur.RoleId)
               .ToListAsync();
    }

    public async Task<List<string>> GetUserRolesAsync(Guid userId)
    {
        return await _dbContext.UserRoles
               .Where(ur => ur.UserId == userId)
               .Select(ur => ur.Role.Name) // EF Core translates this join efficiently
               .ToListAsync();
    }

    public async Task<bool> IsUserInRoleAsync(Guid userId, string roleName)
    {
        return await _dbContext.UserRoles
                .AnyAsync(ur => ur.UserId == userId && ur.Role.Name == roleName);
    }
}
