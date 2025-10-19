using Dotnet.Template.Domain.Entities.Authentication;

namespace Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

public interface IRoleRepository
{
    Task<Role?> GetRoleByNameAsync(string name);
}
