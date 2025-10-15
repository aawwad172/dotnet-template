using Dotnet.Template.Domain.Entities;

namespace Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByUsernameAsync(string username);
}

