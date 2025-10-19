using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Dotnet.Template.Application.Utilities;
using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.Application.Services;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Dotnet.Template.Application.Services;

public class JwtService(
    IConfiguration configuration,
    IPermissionService permissionService,
    ISecurityService securityService,
    IUserRepository userRepository)
    : IJwtService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IPermissionService _permissionService = permissionService;
    private readonly ISecurityService _securityService = securityService;
    private readonly IUserRepository _userRepository = userRepository;
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
            new(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString()), // Unique Token ID
            // --- ADD SECURITY STAMP CLAIM HERE ---
            new("security_stamp", user.SecurityStamp)
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

        string secret = _configuration.GetRequiredSetting("Jwt:JwtSecretKey");
        byte[] keyBytes = TryDecodeBase64(secret) ?? Encoding.UTF8.GetBytes(secret);

        if (keyBytes.Length < 32)
            throw new InvalidOperationException("Jwt:JwtSecretKey must be at least 32 bytes (256-bit).");

        SymmetricSecurityKey key = new(keyBytes);
        SigningCredentials creds = new(key, SecurityAlgorithms.HmacSha256);

        DateTime expires = DateTime.UtcNow.AddMinutes(
                int.TryParse(_configuration.GetRequiredSetting("Jwt:AccessTokenExpirationMinutes"), out int minutes)
                ? minutes
                : throw new InvalidOperationException("Jwt:AccessTokenExpirationMinutes must be a valid integer."));

        SecurityTokenDescriptor tokenDescriptor = new()
        {
            Subject = new ClaimsIdentity(claims),
            Expires = expires,
            SigningCredentials = creds,
            Issuer = _configuration.GetRequiredSetting("Jwt:Issuer"),
            Audience = _configuration.GetRequiredSetting("Jwt:Audience")
        };

        JsonWebTokenHandler handler = new();

        return handler.CreateToken(tokenDescriptor);
    }

    public RefreshToken CreateRefreshTokenEntity(
        User user,
        Guid tokenFamilyId)
    {
        string plaintextToken = GenerateRefreshToken();
        string combinedHashSalt = _securityService.HashSecret(plaintextToken);
        DateTime expiresAt = DateTime.UtcNow.AddDays(
                int.TryParse(_configuration.GetRequiredSetting("Jwt:RefreshTokenExpirationDays"), out int days)
                ? days
                : throw new InvalidOperationException("Jwt:RefreshTokenExpirationDays must be a valid integer."));

        RefreshToken refreshToken = new()
        {
            Id = Guid.CreateVersion7(),
            TokenHash = combinedHashSalt,
            PlaintextToken = plaintextToken,
            UserId = user.Id,
            ExpiresAt = expiresAt,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user.Id,
            TokenFamilyId = tokenFamilyId
        };

        return refreshToken;
    }

    public async Task<ClaimsPrincipal> ValidateToken(string token)
    {
        JsonWebTokenHandler? tokenHandler = new();
        string secret = _configuration.GetRequiredSetting("Jwt:JwtSecretKey");
        byte[] keyBytes = TryDecodeBase64(secret) ?? Encoding.UTF8.GetBytes(secret);

        if (keyBytes.Length < 32)
            throw new InvalidOperationException("Jwt:JwtSecretKey must be at least 32 bytes (256-bit).");

        TokenValidationParameters? validationParameters = new()
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(keyBytes),
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidIssuer = _configuration.GetRequiredSetting("Jwt:Issuer"),
            ValidAudience = _configuration.GetRequiredSetting("Jwt:Audience"),
            ClockSkew = TimeSpan.Zero
        };

        TokenValidationResult result = await tokenHandler.ValidateTokenAsync(token, validationParameters);

        if (!result.IsValid)
        {
            throw new UnauthenticatedException("Invalid token");
        }

        ClaimsPrincipal principal = new(result.ClaimsIdentity);

        // 1. Get User ID and Stamp from the token
        string? userIdClaim = principal.FindFirst(JwtRegisteredClaimNames.NameId)?.Value;
        string? tokenSecurityStamp = principal.FindFirst("security_stamp")?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || string.IsNullOrEmpty(tokenSecurityStamp))
        {
            // Token is missing necessary claims, treat as invalid
            throw new UnauthenticatedException("Token is missing required security claims.");
        }

        if (!Guid.TryParse(userIdClaim, out Guid userId))
            throw new UnauthenticatedException("Token contains invalid user identifier.");

        // 2. Fetch the current user stamp from the database (via a service)
        // NOTE: You will need to implement GetCurrentSecurityStampAsync in your service.
        User? user = await _userRepository.GetByIdAsync(userId);

        if (user is null)
            throw new UnauthenticatedException("Token required security claims is Invalid");

        // 3. Compare the stamps
        if (tokenSecurityStamp != user.SecurityStamp)
            // The stamp was changed (e.g., due to logout or password change)
            throw new UnauthenticatedException("Session revoked. Please log in again.");

        // --- END NEW SECURITY STAMP VALIDATION LOGIC ---

        return principal;
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

    private static byte[]? TryDecodeBase64(string s)
    {
        try
        {
            // tolerate both base64 and base64url
            if (s.Contains('_') || s.Contains('-'))
                return Microsoft.IdentityModel.Tokens.Base64UrlEncoder.DecodeBytes(s);
            return Convert.FromBase64String(s);
        }
        catch { return null; }
    }
    #endregion
}
