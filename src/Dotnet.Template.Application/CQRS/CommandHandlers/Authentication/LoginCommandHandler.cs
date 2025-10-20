using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Entities.Authentication;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.Application.Services;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

using Microsoft.Extensions.Logging;

namespace Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;

public class LoginCommandHandler(
    ICurrentUserService currentUserService,
    ILogger<LoginCommandHandler> logger,
    IUnitOfWork unitOfWork,
    IUserRepository userRepository,
    ISecurityService securityService,
    IRefreshTokenRepository refreshTokenRepository,
    IJwtService jwtService) : BaseHandler<LoginCommand, LoginCommandResult>(currentUserService, logger, unitOfWork)
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly ISecurityService _securityService = securityService;
    private readonly IRefreshTokenRepository _refreshTokenRepository = refreshTokenRepository;
    private readonly IJwtService _jwtService = jwtService;

    public override async Task<LoginCommandResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            User? user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user is null)
                throw new UnauthenticatedException("Invalid email or password.");

            if (user.IsActive is false)
                throw new NotActiveUserException($"User {user.Id} is not active");

            if (user.IsDeleted is true)
                throw new DeletedUserException($"User {user.Id} is deleted");

            if (!_securityService.VerifySecret(
                        secret: request.Password,
                        secretHash: user.PasswordHash
                    )
                )
                throw new UnauthenticatedException("Invalid email or password");

            // All tokens for this session belong to one family ID
            Guid tokenFamilyId = Guid.NewGuid();

            string accessToken = await _jwtService.GenerateAccessTokenAsync(user);

            RefreshToken refreshToken = _jwtService.CreateRefreshTokenEntity(user, tokenFamilyId);

            await _refreshTokenRepository.AddAsync(refreshToken);

            user.RefreshTokens.Add(refreshToken);

            // --- REMOVE THE REDUNDANT CALL ---
            // await _userRepository.UpdateAsync(user); // <--- REMOVE THIS LINE!!!
            // ----------------------------------

            await _unitOfWork.SaveAsync();
            await _unitOfWork.CommitAsync();

            return new LoginCommandResult(AccessToken: accessToken, RefreshToken: refreshToken.PlaintextToken);
        }
        catch (UnauthenticatedException)
        {
            await _unitOfWork.RollbackAsync();
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred during login.");
            await _unitOfWork.RollbackAsync();
            // Preserve generic message but use the domain exception type used elsewhere.
            throw new UnauthenticatedException("Invalid email or password.");
        }
    }
}
