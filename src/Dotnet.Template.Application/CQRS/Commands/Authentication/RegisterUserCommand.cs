using Dotnet.Template.Domain.Entities;

using MediatR;

namespace Dotnet.Template.Application.CQRS.Commands.Authentication;

public record RegisterUserCommand(
    string FirstName,
    string LastName,
    string Email,
    string Username,
    string Password) : IRequest<RegisterUserCommandResult>;

public record RegisterUserCommandResult(User user, string message);
