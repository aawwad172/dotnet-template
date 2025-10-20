using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Interfaces.Application.Services;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

using Microsoft.EntityFrameworkCore;

namespace Dotnet.Template.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository(
    ApplicationDbContext context,
    ISecurityService securityService)
    : Repository<RefreshToken>(context), IRefreshTokenRepository
{
    private readonly ISecurityService _securityService = securityService;
    public async Task<RefreshToken?> GetByTokenAsync(string token, Guid userId)
    {
        // 1. --- Efficient Database Fetch (Filter first, then fetch necessary data) ---
        // Fetch only active, non-expired tokens belonging to this user.
        // We fetch the HASH and SALT which are required for verification.
        List<RefreshToken> candidateTokens = await _dbSet
            .Where(rt => rt.UserId == userId
                        && !rt.RevokedAt.HasValue
                        && rt.ExpiresAt > DateTime.UtcNow).ToListAsync();

        // If the user has no tokens, exit early.
        if (!candidateTokens.Any())
            return null;

        // 2. --- In-Memory Verification (Cannot be done in SQL) ---
        // Iterate through candidates and verify the hash using the SecurityService.
        RefreshToken? verifiedToken = candidateTokens.FirstOrDefault(rt => _securityService.VerifySecret(token, rt.TokenHash));

        // 3. --- Return ---
        return verifiedToken;
    }
}
