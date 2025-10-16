using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Interfaces.Application.Services;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

namespace Dotnet.Template.Application.Services;

public class PermissionService(IAuthenticationRepository authenticationRepository) : IPermissionService
{
    private readonly IAuthenticationRepository _authenticationRepository = authenticationRepository;

    public async Task<List<string>> GetUserPermissionsAsync(User user)
    {
        // 1. --- Check for SuperAdmin (Business Rule) ---
        // Uses the repository to check if the user is a SuperAdmin.
        if (await _authenticationRepository.IsUserInRoleAsync(user.Id, "SuperAdmin"))
        {
            // SuperAdmin bypass: return all permissions defined in the system.
            return await _authenticationRepository.GetAllPermissionNamesAsync();
        }

        // 2. --- Calculate Base Permissions (from Roles) ---
        List<Guid> userRoleIds = await _authenticationRepository.GetUserRoleIdsAsync(user.Id);
        List<string> baseGrantedPermissions = await _authenticationRepository.GetBaseGrantedPermissionsAsync(userRoleIds.ToList());

        HashSet<string> finalPermissions = new(baseGrantedPermissions, StringComparer.OrdinalIgnoreCase);

        return finalPermissions.ToList();
    }

    public async Task<List<string>> GetUserRolesAsync(Guid userId)
    {
        // Simple passthrough to the repository
        return await _authenticationRepository.GetUserRolesAsync(userId);
    }
}
