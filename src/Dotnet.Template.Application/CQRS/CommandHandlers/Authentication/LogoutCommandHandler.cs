
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Application.Interfaces.Services;

using MediatR;

namespace Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;

public class LogoutCommandHandler(IAuthenticationService authService) : IRequestHandler<LogoutCommand, LogoutResult>
{
    private readonly IAuthenticationService _authService = authService;
    public async Task<LogoutResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(request.RefreshToken!);

        return new LogoutResult("Logout successful");
    }
}
