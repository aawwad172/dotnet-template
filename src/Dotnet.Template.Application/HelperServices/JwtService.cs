using System.Security.Claims;
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

    public string GenerateToken(User user, DateTime expires)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetRequiredSetting("Jwt:JwtSecretKey")));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(
            [
                new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Jti, Ulid.NewUlid().ToString())
            ]),
            Expires = expires,
            SigningCredentials = creds,
            Issuer = _configuration.GetRequiredSetting("Jwt:Issuer"),
            Audience = _configuration.GetRequiredSetting("Jwt:Audience")
        };


        JsonWebTokenHandler? handler = new JsonWebTokenHandler();

        return handler.CreateToken(tokenDescriptor);
    }

    public string GenerateRefreshToken()
    {
        return _encryptionService.GenerateRandomString();
    }

    public bool VerifyRefreshToken(string refreshToken, string storedHash)
    {
        string hashedInput = _encryptionService.Hash(refreshToken);
        return storedHash == hashedInput;
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

        var result = await tokenHandler.ValidateTokenAsync(token, validationParameters);
        return result.IsValid ? new ClaimsPrincipal(result.ClaimsIdentity) : null!;
    }
}
