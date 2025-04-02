using System.Security.Authentication;

using Dotnet.Template.Application.Interfaces.Services;
using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Enums;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.IRepositories;

using Microsoft.Extensions.Logging;

namespace Dotnet.Template.Application.HelperServices;

public sealed class AuthenticationService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtService jwtService,
    IEncryptionService encryptionService,
    IUnitOfWork unitOfWork,
    ILogger<AuthenticationService> logger) : BaseService<AuthenticationService>(logger), IAuthenticationService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IEncryptionService _encryptionService = encryptionService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<User> RegisterUserAsync(
        string firstName,
        string lastName,
        string email,
        string username,
        string password)
    {
        // Check if a user already exists with the same email.
        User? existingUser = await _userRepository.GetUserByEmailAsync(email);
        if (existingUser is not null)
            throw new ConflictException("A user with this email already exists.");

        // Check if a user already exists with the same username.
        User? existingUsername = await _userRepository.GetUserByUsernameAsync(username);
        if (existingUsername is not null)
            throw new ConflictException("A user with this username already exists.");

        Guid id = Guid.CreateVersion7();

        User user = new User
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Username = username,
            PasswordHash = _encryptionService.HashPassword(password),
            Role = RolesEnum.User,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty,
        };

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            _ = _userRepository.AddAsync(user);
            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitAsync();

            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during login: {Message}", ex.Message);
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }


    public async Task<(string AccessToken, string RefreshToken)> LoginAsync(string email, string password)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            User? user = await _userRepository.GetUserByEmailAsync(email);
            if (user is null)
                throw new NotFoundException("User not found");

            if (!_encryptionService.VerifyPassword(
                        password: password,
                        passwordHash: user.PasswordHash
                    )
                )
                throw new UnauthenticatedException("Invalid password");

            string accessToken = _jwtService.GenerateAccessToken(user);

            RefreshToken refreshToken = _jwtService.GenerateRefreshToken(user);

            string unhashedRefreshToken = refreshToken.Token;
            // Hash the refresh token before storing it
            refreshToken.Token = _encryptionService.Hash(unhashedRefreshToken);
            user.RefreshTokens.Add(refreshToken);

            // Updating the User in the DB with the new Refresh Token
            await _userRepository.UpdateAsync(user);

            _ = await _refreshTokenRepository.AddAsync(refreshToken);

            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitAsync();

            return (accessToken, unhashedRefreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during login: {Message}", ex.Message);
            await _unitOfWork.RollbackAsync();
            throw new AuthenticationException("Invalid email or password");
        }
    }

    public async Task LogoutAsync(string refreshToken)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            string hashedToken = _encryptionService.Hash(refreshToken);

            RefreshToken? token = await _refreshTokenRepository.GetByTokenAsync(hashedToken);

            if (token is null || !token.IsActive)
                throw new UnauthorizedException("Invalid refresh token");

            token.Revoked = DateTime.UtcNow;

            // Update the token entity in the repository.
            await _refreshTokenRepository.UpdateAsync(token!);

            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitAsync();
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during login: {Message}", ex.Message);
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }

    public async Task<(string AccessToken, string RefreshToken)> RefreshAccessTokenAsync(string refreshToken)
    {
        string hashedToken = _encryptionService.Hash(refreshToken);

        RefreshToken? token = await _refreshTokenRepository.GetByTokenAsync(hashedToken);

        if (token is null || !token.IsActive)
            throw new UnauthenticatedException("Invalid or expired refresh token.");


        User? user = await _userRepository.GetByIdAsync(token.UserId);
        if (user is null)
            throw new NotFoundException("User not found");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            //  Generate a new refresh token
            RefreshToken newRefreshToken = _jwtService.GenerateRefreshToken(user);

            token.Revoked = DateTime.UtcNow;
            token.ReplacedByToken = newRefreshToken.Id;

            await _refreshTokenRepository.UpdateAsync(token);

            string unhashedNewToken = newRefreshToken.Token;

            // Hash the new refresh token
            newRefreshToken.Token = _encryptionService.Hash(unhashedNewToken);

            // Add the new refresh token to the repository
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            // Generate a new access token with desired expiration
            string newAccessToken = _jwtService.GenerateAccessToken(user);

            // Save all changes and commit the transaction
            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitAsync();

            // Return the new tokens to the client
            return (newAccessToken, unhashedNewToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during login: {Message}", ex.Message);
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
