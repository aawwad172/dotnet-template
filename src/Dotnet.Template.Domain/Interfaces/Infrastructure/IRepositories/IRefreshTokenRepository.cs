using Dotnet.Template.Domain.Entities.Authentication;

namespace Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token, Guid userId);
}
