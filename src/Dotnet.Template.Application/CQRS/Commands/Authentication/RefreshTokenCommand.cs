using MediatR;

namespace Dotnet.Template.Application.CQRS.Commands.Authentication;

public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenCommandResult>;

public record RefreshTokenCommandResult(string AccessToken, string RefreshToken);
