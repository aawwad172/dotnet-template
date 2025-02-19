using System.Security.Claims;

using Dotnet.Template.Domain.Entities;

namespace Dotnet.Template.Application.Interfaces.Services;

public interface IJwtService
{
    string GenerateToken(User user, DateTime expires);
    public string GenerateRefreshToken();
    Task<ClaimsPrincipal> ValidateToken(string token);
    public bool VerifyRefreshToken(string refreshToken, string storedHash);
}
