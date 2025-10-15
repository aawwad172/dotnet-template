
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Domain.Interfaces.Application.Services;

namespace Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;

public class LogoutCommandHandler(
    IAuthenticationService authService,
    ICurrentUserService currentUserService)
    : BaseHandler<LogoutCommand, LogoutCommandResult>(currentUserService)
{
    private readonly IAuthenticationService _authService = authService;

    public override async Task<LogoutCommandResult> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        await _authService.LogoutAsync(request.RefreshToken!, _currentUser.UserId);

        return new LogoutCommandResult("Logout successful");
    }
}
