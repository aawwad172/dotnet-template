using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Dotnet.Template.Application.Utilities;
using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.Application.Services;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Dotnet.Template.Application.Services;

public class JwtService(
    IConfiguration configuration,
    IPermissionService permissionService,
    ISecurityService securityService)
    : IJwtService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IPermissionService _permissionService = permissionService;
    private readonly ISecurityService _securityService = securityService;
    public async Task<string> GenerateAccessTokenAsync(User user)
    {
        // --- 1. Fetch ALL necessary claims from the service layer ---
        List<string> roles = await _permissionService.GetUserRolesAsync(user.Id);
        List<string> permissions = await _permissionService.GetUserPermissionsAsync(user);

        // Use a List<Claim> to dynamically add all claims
        var claims = new List<Claim>
        {
            // Core Identity Claims
            new(JwtRegisteredClaimNames.Name, user.FirstName + " " + user.LastName),
            new(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
            new(JwtRegisteredClaimNames.UniqueName, user.Username),
            new(JwtRegisteredClaimNames.Email, user.Email),
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()) // Unique Token ID
        };

        // --- 3. Add Role Claims ---
        foreach (var role in roles)
        {
            // Add one claim for each role the user holds
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Use the custom claim type defined earlier (CustomClaims.Permission = "application_permission")
        // --- 4. Add Permission Claims (for granular [HasPermission("...")] checks) ---
        foreach (var permission in permissions)
        {
            // Use the custom claim type defined earlier (e.g., CustomClaims.Permission)
            claims.Add(new Claim(CustomClaims.Permission, permission));
        }

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration.GetRequiredSetting("Jwt:JwtSecretKey")));
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetRequiredSetting("Jwt:AccessTokenExpirationMinutes"))),
            SigningCredentials = creds,
            Issuer = _configuration.GetRequiredSetting("Jwt:Issuer"),
            Audience = _configuration.GetRequiredSetting("Jwt:Audience")
        };

        JsonWebTokenHandler handler = new();

        return handler.CreateToken(tokenDescriptor);
    }

    public RefreshToken CreateRefreshTokenEntityAsync(
        User user,
        Guid tokenFamilyId)
    {
        string plaintextToken = GenerateRefreshToken();
        string combinedHashSalt = _securityService.HashSecret(plaintextToken);
        RefreshToken refreshToken = new()
        {
            Id = Guid.CreateVersion7(),
            TokenHash = combinedHashSalt,
            PlaintextToken = plaintextToken,
            UserId = user.Id,
            ExpiresAt = DateTime.UtcNow.AddDays(int.Parse(_configuration.GetRequiredSetting("Jwt:RefreshTokenExpirationDays"))),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user.Id,
            TokenFamilyId = tokenFamilyId
        };

        return refreshToken;
    }

    public async Task<ClaimsPrincipal> ValidateToken(string token)
    {
        JsonWebTokenHandler? tokenHandler = new();
        byte[]? key = Encoding.UTF8.GetBytes(_configuration.GetRequiredSetting("Jwt:JwtSecretKey"));

        TokenValidationParameters? validationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidIssuer = _configuration.GetRequiredSetting("Jwt:Issuer"),
            ValidAudience = _configuration.GetRequiredSetting("Jwt:Audience"),
            ClockSkew = TimeSpan.Zero
        };

        TokenValidationResult result = await tokenHandler.ValidateTokenAsync(token, validationParameters);

        if (!result.IsValid)
        {
            throw new UnauthenticatedException("Invalid token");
        }

        return new ClaimsPrincipal(result.ClaimsIdentity);
    }

    #region Private Helper Methods
    private string GenerateRefreshToken()
    {
        byte[] randomBytes = new byte[32]; // 256-bit secure random token
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return Convert.ToBase64String(randomBytes);
    }
    #endregion
}
