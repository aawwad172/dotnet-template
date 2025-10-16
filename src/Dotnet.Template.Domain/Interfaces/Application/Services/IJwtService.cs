using System.Security.Claims;

using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Entities.Authentication;

namespace Dotnet.Template.Domain.Interfaces.Application.Services;

public interface IJwtService
{
    Task<string> GenerateAccessTokenAsync(User user);
    RefreshToken CreateRefreshTokenEntity(
        User user,
        Guid tokenFamilyId);
    Task<ClaimsPrincipal> ValidateToken(string token);
}
