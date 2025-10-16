
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.Application.Services;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

using Microsoft.Extensions.Logging;

namespace Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;

public class LogoutCommandHandler(
    ICurrentUserService currentUserService,
    ILogger<LogoutCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IRefreshTokenRepository refreshTokenRepository,
    ISecurityService securityService,
    IUserRepository userRepository)
    : BaseHandler<LogoutCommand, LogoutCommandResult>(currentUserService, logger, unitOfWork)
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly ISecurityService _securityService = securityService;
    private readonly IUserRepository _userRepository = userRepository;

    public override async Task<LogoutCommandResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            // 1. --- Fetch Candidate Token (by Plaintext Token) ---
            // CRITICAL: The repository must find the token by the PLAINTEXT value.
            // The repository will fetch the entity based on the PLAINTEXT token value 
            // by verifying the hash against the TokenHash/TokenSalt in the DB.

            // This method must perform the hashing/lookup logic.
            RefreshToken? token = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken!, _currentUser.UserId);

            if (token is null || token.IsRevoked || token.IsExpired)
                throw new UnauthorizedException("Session is already invalid or token not found.");


            // 2. --- Token Verification (Crucial Security Check) ---
            // Verify the incoming plaintext token matches the stored HASH/SALT.
            // This is done by the service, using the retrieved token's salt.
            bool isVerified = _securityService.VerifySecret(request.RefreshToken!, token.TokenHash);

            if (!isVerified)
                // This suggests a token collision or tampering, treat as unauthorized.
                throw new UnauthorizedException("Invalid refresh token signature.");


            // 3. --- Revoke and Audit ---
            token.RevokedAt = DateTime.UtcNow; // Corrected property name
            token.ReasonRevoked = "Manual Logout";

            User? user = await _userRepository.GetByIdAsync(_currentUser.UserId);

            if (user is null)
                throw new NotFoundException("Cannot find user");


            // Mark the User as updated (optional, but good practice)
            user.UpdatedAt = DateTime.UtcNow;
            user.SecurityStamp = Guid.NewGuid().ToString(); // Optional: Revoke all other sessions

            // Update the token entity in the repository.
            await _refreshTokenRepository.UpdateAsync(token!);

            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitAsync();
            return new LogoutCommandResult("Logout successful");
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during logout: {Message}", ex.Message);
            await _unitOfWork.RollbackAsync();
            throw;
        }

    }
}
