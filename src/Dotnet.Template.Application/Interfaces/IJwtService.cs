using System.Security.Claims;

namespace Dotnet.Template.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(IEnumerable<Claim> claims, DateTime expires);
    ClaimsPrincipal ValidateToken(string token);
}
