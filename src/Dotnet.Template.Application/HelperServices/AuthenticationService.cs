using Dotnet.Template.Application.Interfaces.Services;
using Dotnet.Template.Application.Utilities;
using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.IRepositories;

using Microsoft.Extensions.Configuration;

namespace Dotnet.Template.Application.HelperServices;

public sealed class AuthenticationService(IUserRepository userRepository, IRepository<RefreshToken> refreshTokenRepository, IJwtService jwtService, IEncryptionService encryptionService, IConfiguration configuration, IUnitOfWork unitOfWork) : IAuthenticationService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRepository<RefreshToken> _refreshTokenRepository = refreshTokenRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IConfiguration _configuration = configuration;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;


    public async Task<(string AccessToken, string RefreshToken)> AuthenticateUserAsync(string email, string password)
    {
        // Todo: Consider using the UnitOfWork pattern to manage transactions
        User? user = await _userRepository.GetUserByEmailAsync(email);

        if (user is null)
        {
            throw new NotFoundException("User not found");
        }

        if (!_encryptionService.VerifyPassword(password, user.PasswordHash))
        {
            throw new UnauthenticatedException("Invalid password");
        }

        string accessToken = _jwtService.GenerateToken(user, DateTime.UtcNow.AddMinutes(int.Parse(_configuration.GetRequiredSetting("Jwt:AccessTokenExpirationMinutes"))));

        string refreshTokenValue = _jwtService.GenerateRefreshToken();

        RefreshToken refreshToken = await _refreshTokenRepository.AddAsync(new RefreshToken
        {
            Token = refreshTokenValue,
            UserId = user.Id,
            Expires = DateTime.UtcNow.AddDays(int.Parse(_configuration.GetRequiredSetting("Jwt:RefreshTokenExpirationDays"))),
            CreatedAt = DateTime.UtcNow,
        });

        return (accessToken, refreshToken.Token);
    }

    public Task LogoutAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }

    public Task<(string AccessToken, string RefreshToken)> RefreshAccessTokenAsync(string refreshToken)
    {
        throw new NotImplementedException();
    }
}
