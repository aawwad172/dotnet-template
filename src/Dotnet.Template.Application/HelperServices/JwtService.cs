using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Dotnet.Template.Application.Interfaces;
using Dotnet.Template.Application.Utilities;

using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Dotnet.Template.Application.HelperServices;

public class JwtService(IConfiguration configuration) : IJwtService
{
    private readonly IConfiguration _configuration = configuration;
    public string GenerateToken(IEnumerable<Claim> claims, DateTime expires)
    {
        SymmetricSecurityKey key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetRequiredSetting("Jwt:Key")));
        SigningCredentials creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: _configuration.GetRequiredSetting("Jwt:Issuer"),
            audience: _configuration.GetRequiredSetting("Jwt:Audience"),
            claims: claims,
            expires: expires,
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public ClaimsPrincipal ValidateToken(string token)
    {
        try
        {
            JwtSecurityTokenHandler? tokenHandler = new JwtSecurityTokenHandler();
            byte[]? key = Encoding.UTF8.GetBytes(_configuration.GetRequiredSetting("JwtSecretKey"));

            TokenValidationParameters? validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration.GetRequiredSetting("Jwt:Issuer"),
                ValidAudience = _configuration.GetRequiredSetting("Jwt:Audience"),
                ClockSkew = TimeSpan.Zero
            };

            return tokenHandler.ValidateToken(token, validationParameters, out _);

        }
        catch
        {
            // Token is invalid or expired
            return null!;
        }

    }
}
