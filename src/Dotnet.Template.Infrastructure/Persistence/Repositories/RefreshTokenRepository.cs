using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.IRepositories;

using Microsoft.EntityFrameworkCore;

namespace Dotnet.Template.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(BaseDbContext context) : Repository<RefreshToken>(context), IRefreshTokenRepository
{
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _dbSet.FirstOrDefaultAsync(row => row.Token == token);
    }
}
