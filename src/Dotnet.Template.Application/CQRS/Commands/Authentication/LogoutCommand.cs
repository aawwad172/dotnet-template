using MediatR;

namespace Dotnet.Template.Application.CQRS.Commands.Authentication;

public record LogoutCommand(string? RefreshToken) : IRequest<LogoutCommandResult>;

public record LogoutCommandResult(string Message);
