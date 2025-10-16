
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.Application.Services;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

using Microsoft.Extensions.Logging;

namespace Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;

public class RefreshTokenCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<RefreshTokenCommandHandler> logger,
    ICurrentUserService currentUserService,
    IRefreshTokenRepository refreshTokenRepository,
    IUserRepository userRepository,
    IJwtService jwtService)
    : BaseHandler<RefreshTokenCommand, RefreshTokenCommandResult>(currentUserService, logger, unitOfWork)
{
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;

    public async override Task<RefreshTokenCommandResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        RefreshToken? oldToken = await _refreshTokenRepository.GetByTokenAsync(request.RefreshToken, _currentUser.UserId);

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
            return new RefreshTokenCommandResult(newAccessToken, newRefreshToken.PlaintextToken);
        }
        catch (Exception ex)
        {
            _logger.LogError("An error occurred during login: {Message}", ex.Message);
            await _unitOfWork.RollbackAsync();
            throw;
        }

    }
}
