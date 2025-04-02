
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Application.Interfaces.Services;

using MediatR;

namespace Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;

public class RefreshTokenCommandHandler(IAuthenticationService authService) : IRequestHandler<RefreshTokenCommand, RefreshTokenResult>
{
    private readonly IAuthenticationService _authService = authService;

    public async Task<RefreshTokenResult> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        (string accessToken, string refreshToken) = await _authService.RefreshAccessTokenAsync(request.RefreshToken);

        return new RefreshTokenResult(accessToken, refreshToken);
    }
}
