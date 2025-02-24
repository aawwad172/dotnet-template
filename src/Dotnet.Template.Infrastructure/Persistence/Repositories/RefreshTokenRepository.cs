using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.IRepositories;

using Microsoft.EntityFrameworkCore;

namespace Dotnet.Template.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(BaseDbContext context) : Repository<RefreshToken>(context), IRefreshTokenRepository
{
    private readonly DbSet<RefreshToken> _dbSet = context.Set<RefreshToken>();
    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        RefreshToken? refreshToken = await _dbSet.Where(row => row.Token == token).FirstOrDefaultAsync();

        if (refreshToken is not null)
            return refreshToken;

        return null;
    }
}
