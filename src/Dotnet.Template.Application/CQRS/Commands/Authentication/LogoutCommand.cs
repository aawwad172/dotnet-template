using MediatR;

namespace Dotnet.Template.Application.CQRS.Commands.Authentication;

public record LogoutCommand(string? RefreshToken) : IRequest<LogoutResult>;

public record LogoutResult(string message);
