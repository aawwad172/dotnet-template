using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

using Dotnet.Template.Application.Interfaces.Services;
using Dotnet.Template.Application.Utilities;
using Dotnet.Template.Domain.Entities;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Dotnet.Template.Application.HelperServices;

public class JwtService(IConfiguration configuration, IEncryptionService encryptionService) : IJwtService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly IEncryptionService _encryptionService = encryptionService;

    public string GenerateAccessToken(User user)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetRequiredSetting("Jwt:JwtSecretKey")));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Name, user.FirstName + " " + user.LastName),
                new Claim(JwtRegisteredClaimNames.NameId, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.CreateVersion7().ToString())
            ]),
            Expires = DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetRequiredSetting("Jwt:AccessTokenExpirationMinutes"))),
            SigningCredentials = creds,
            Issuer = _configuration.GetRequiredSetting("Jwt:Issuer"),
            Audience = _configuration.GetRequiredSetting("Jwt:Audience")
        };

        JsonWebTokenHandler handler = new JsonWebTokenHandler();

        return handler.CreateToken(tokenDescriptor);
    }

    public RefreshToken GenerateRefreshToken(User user)
    {
        RefreshToken refreshToken = new RefreshToken
        {
            Token = GenerateRefreshToken(),
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(int.Parse(_configuration.GetRequiredSetting("Jwt:RefreshTokenExpirationDays"))),
            CreatedAt = DateTime.UtcNow,
            CreatedBy = user.Id
        };

        user.RefreshTokens.Add(refreshToken);

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
        return result.IsValid ? new ClaimsPrincipal(result.ClaimsIdentity) : null!;
    }

    #region Private Helper Methods
    private string GenerateRefreshToken()
    {
        byte[] randomBytes = new byte[32]; // 256-bit secure random token
        using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
        {
            rng.GetBytes(randomBytes);
        }
        return _encryptionService.Hash(Convert.ToBase64String(randomBytes));
    }
    #endregion
}
