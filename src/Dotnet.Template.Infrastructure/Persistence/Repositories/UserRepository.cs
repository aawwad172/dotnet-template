using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.IRepositories;

using Microsoft.EntityFrameworkCore;

namespace Dotnet.Template.Infrastructure.Persistence.Repositories;

public sealed class UserRepository(BaseDbContext dbContext) : Repository<User>(dbContext), IUserRepository
{
    private readonly DbSet<User> _dbSet = dbContext.Set<User>();

    public Task<User> GetUserByEmailAsync(string email)
    {
        Task<User?>? user = _dbSet.FirstOrDefaultAsync(user => user.Email == email);

        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        return user!;
    }
}
