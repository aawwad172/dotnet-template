using Dotnet.Template.Domain.Entities.Authentication;

namespace Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

public interface IAuthenticationRepository
{
    Task<List<string>> GetUserRolesAsync(Guid userId);

    // Permission Retrieval
    Task<List<string>> GetBaseGrantedPermissionsAsync(List<Guid> roleIds);
    // Helper
    Task<List<Guid>> GetUserRoleIdsAsync(Guid userId);
    Task<bool> IsUserInRoleAsync(Guid userId, string roleName);
    Task<List<string>> GetAllPermissionNamesAsync();
    Task AddUserRoleAsync(UserRole userRole);
}
