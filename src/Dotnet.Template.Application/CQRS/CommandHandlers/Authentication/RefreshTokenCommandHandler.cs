
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Domain.Interfaces.Application.Services;

namespace Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;

public class RefreshTokenCommandHandler(
    IAuthenticationService authService,
    ICurrentUserService currentUserService)
    : BaseHandler<RefreshTokenCommand, RefreshTokenCommandResult>(currentUserService)
{
    private readonly IAuthenticationService _authService = authService;

    public async override Task<RefreshTokenCommandResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        (string accessToken, string refreshToken) = await _authService.RefreshAccessTokenAsync(request.RefreshToken, _currentUser.UserId);

        return new RefreshTokenCommandResult(accessToken, refreshToken);
    }
}
