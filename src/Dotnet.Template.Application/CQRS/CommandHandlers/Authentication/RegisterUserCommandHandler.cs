
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Application.Interfaces.Services;

using MediatR;

namespace Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;

public class RegisterUserCommandHandler(IAuthenticationService authService) : IRequestHandler<RegisterUserCommand, RegisterUserResult>
{
    private readonly IAuthenticationService _authService = authService;

    public async Task<RegisterUserResult> Handle(RegisterUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _authService.RegisterUserAsync(
            firstName: request.FirstName,
            lastName: request.LastName,
            email: request.Email,
            password: request.Password,
            username: request.Username);

        return new RegisterUserResult(user, "User registered successfully");
    }
}
