using System.Security.Claims;

using Dotnet.Template.Domain.Entities;

namespace Dotnet.Template.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    public RefreshToken GenerateRefreshToken(User user);
    Task<ClaimsPrincipal> ValidateToken(string token);
}
