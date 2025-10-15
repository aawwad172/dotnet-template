
using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

using Microsoft.EntityFrameworkCore;

namespace Dotnet.Template.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(BaseDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
    public async Task<User?> GetUserByEmailAsync(string email)
    {
        User? user = await _dbSet.FirstOrDefaultAsync(user => user.Email == email);

        return user is not null ? user : null;
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        User? user = await _dbSet.FirstOrDefaultAsync(user => user.Username == username);

        return user is not null ? user : null;
    }
}
