using MediatR;

namespace Dotnet.Template.Application.CQRS.Commands.Authentication;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Password) : IRequest<RegisterUserCommandResult>;

public record RegisterUserCommandResult(
    Guid Id,
    string FirstName,
    string LastName,
    string Email,
    string Username,
    bool IsActive,
    bool IsVerified,
    string Message
);
