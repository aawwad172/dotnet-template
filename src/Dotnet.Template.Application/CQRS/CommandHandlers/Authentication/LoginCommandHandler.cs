
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Application.Interfaces.Services;

using MediatR;

namespace Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;

public class LoginCommandHandler(IAuthenticationService authService) : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IAuthenticationService _authService = authService;
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var (accessToken, refreshToken) = await _authService.LoginAsync(
            email: request.email,
            password: request.password
        );

        return new LoginResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken
        );
    }
}
