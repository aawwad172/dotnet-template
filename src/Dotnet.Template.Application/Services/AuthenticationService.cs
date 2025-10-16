using System.Security.Authentication;

using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.Application.Services;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

using Microsoft.Extensions.Logging;

namespace Dotnet.Template.Application.Services;

public sealed class AuthenticationService(
    IUserRepository userRepository,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtService jwtService,
    ISecurityService securityService,
    IUnitOfWork unitOfWork,
    IRoleRepository roleRepository,
    IAuthenticationRepository authenticationRepository,
    ILogger<AuthenticationService> logger)
    : BaseService<AuthenticationService>(logger), IAuthenticationService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ISecurityService _securityService = securityService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IRoleRepository _roleRepository = roleRepository;
    private readonly IAuthenticationRepository _authenticationRepository = authenticationRepository;
    private readonly string _defaultRoleName = "User";


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

        // 1. Hash the password (returns combined HASH-SALT string)
        string hashedPassword = _securityService.HashSecret(password);

        // 2. Split the combined string into its two secure parts
        string securityStamp = Guid.NewGuid().ToString();

        Guid id = Guid.CreateVersion7();

        Role? defaultRole = await _roleRepository.GetRoleByNameAsync(_defaultRoleName);

        if (defaultRole is null)
        {
            // IMPORTANT: This prevents users from being created without a role if the DB isn't seeded.
            throw new InvalidOperationException($"The default role '{_defaultRoleName}' does not exist in the database. Please seed roles.");
        }

        User user = new()
        {
            Id = id,
            FirstName = firstName,
            LastName = lastName,
            Email = email,
            Username = username,
            PasswordHash = hashedPassword,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty,
            IsActive = false,
            IsDeleted = false,
            IsVerified = false,
            SecurityStamp = securityStamp,
        };

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            _ = _userRepository.AddAsync(user);

            UserRole userRole = new()
            {
                UserId = user.Id,
                RoleId = defaultRole.Id,
            };
            await _authenticationRepository.AddUserRoleAsync(userRole); // Assuming IAuthRepository has this method

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


    public async Task<(string AccessToken, string RefreshToken)> LoginAsync(
        string email,
        string password)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            User? user = await _userRepository.GetUserByEmailAsync(email);
            if (user is null)
                throw new UnauthenticatedException("Invalid email or password.");

            if (!_securityService.VerifySecret(
                        secret: password,
                        secretHash: user.PasswordHash
                    )
                )
                throw new UnauthenticatedException("Invalid email or password");

            // All tokens for this session belong to one family ID
            Guid tokenFamilyId = Guid.NewGuid();

            string accessToken = await _jwtService.GenerateAccessTokenAsync(user);

            RefreshToken refreshToken = _jwtService.CreateRefreshTokenEntityAsync(user, tokenFamilyId);

            await _refreshTokenRepository.AddAsync(refreshToken);

            // Hash the refresh token before storing it
            user.RefreshTokens.Add(refreshToken);

            // Updating the User in the DB with the new Refresh Token
            await _userRepository.UpdateAsync(user);

            _ = await _refreshTokenRepository.AddAsync(refreshToken);

            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitAsync();

            return (accessToken, refreshToken.PlaintextToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during login: {Message}", ex.Message);
            await _unitOfWork.RollbackAsync();
            throw new AuthenticationException("Invalid email or password");
        }
    }

    public async Task LogoutAsync(
        string refreshToken,
        Guid userId)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. --- Fetch Candidate Token (by Plaintext Token) ---
            // CRITICAL: The repository must find the token by the PLAINTEXT value.
            // The repository will fetch the entity based on the PLAINTEXT token value 
            // by verifying the hash against the TokenHash/TokenSalt in the DB.

            // This method must perform the hashing/lookup logic.
            RefreshToken? token = await _refreshTokenRepository.GetByTokenAsync(refreshToken, userId);

            if (token is null || token.IsRevoked || token.IsExpired)
                throw new UnauthorizedException("Session is already invalid or token not found.");


            // 2. --- Token Verification (Crucial Security Check) ---
            // Verify the incoming plaintext token matches the stored HASH/SALT.
            // This is done by the service, using the retrieved token's salt.
            bool isVerified = _securityService.VerifySecret(refreshToken, token.TokenHash);

            if (!isVerified)
                // This suggests a token collision or tampering, treat as unauthorized.
                throw new UnauthorizedException("Invalid refresh token signature.");


            // 3. --- Revoke and Audit ---
            token.RevokedAt = DateTime.UtcNow; // Corrected property name
            token.ReasonRevoked = "Manual Logout";

            User? user = await _userRepository.GetByIdAsync(userId);

            if (user is null)
                throw new NotFoundException("Cannot find user");


            // Mark the User as updated (optional, but good practice)
            user.UpdatedAt = DateTime.UtcNow;
            user.SecurityStamp = Guid.NewGuid().ToString(); // Optional: Revoke all other sessions

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

    public async Task<(string AccessToken, string RefreshToken)> RefreshAccessTokenAsync(string refreshToken, Guid userId)
    {
        RefreshToken? oldToken = await _refreshTokenRepository.GetByTokenAsync(refreshToken, userId);

        if (oldToken is null || !oldToken.IsActive)
            throw new UnauthenticatedException("Invalid or expired refresh token.");


        User? user = await _userRepository.GetByIdAsync(oldToken.UserId);
        if (user is null)
            throw new NotFoundException("User not found");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            //  Generate a new refresh token
            RefreshToken newRefreshToken = _jwtService.CreateRefreshTokenEntityAsync(user, oldToken.TokenFamilyId);

            oldToken.RevokedAt = DateTime.UtcNow;
            oldToken.ReasonRevoked = "Rotated";
            oldToken.ReplacedByTokenId = newRefreshToken.Id; // CRITICAL: Link to the new token ID

            await _refreshTokenRepository.UpdateAsync(oldToken);

            // Add the new refresh token to the repository
            await _refreshTokenRepository.AddAsync(newRefreshToken);

            // Generate a new access token with desired expiration
            string newAccessToken = await _jwtService.GenerateAccessTokenAsync(user);

            // Save all changes and commit the transaction
            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitAsync();

            // 4. --- Return New Tokens ---
            // newRefreshTokenEntity.PlaintextToken is available because of the [NotMapped] property.
            return (newAccessToken, newRefreshToken.PlaintextToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during login: {Message}", ex.Message);
            await _unitOfWork.RollbackAsync();
            throw;
        }
    }
}
