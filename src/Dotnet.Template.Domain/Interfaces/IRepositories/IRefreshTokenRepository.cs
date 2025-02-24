using Dotnet.Template.Domain.Entities;

namespace Dotnet.Template.Domain.Interfaces.IRepositories;

public interface IRefreshTokenRepository : IRepository<RefreshToken>
{
    Task<RefreshToken?> GetByTokenAsync(string token);
}
