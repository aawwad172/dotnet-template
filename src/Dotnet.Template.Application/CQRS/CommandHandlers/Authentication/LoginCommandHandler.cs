
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Domain.Interfaces.Application.Services;

using MediatR;

namespace Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;

public class LoginCommandHandler(IAuthenticationService authService) : IRequestHandler<LoginCommand, LoginResult>
{
    private readonly IAuthenticationService _authService = authService;
    public async Task<LoginResult> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        (string accessToken, string refreshToken) = await _authService.LoginAsync(
            email: request.Email,
            password: request.Password
        );



        return new LoginResult(
            AccessToken: accessToken,
            RefreshToken: refreshToken
        );
    }
}
