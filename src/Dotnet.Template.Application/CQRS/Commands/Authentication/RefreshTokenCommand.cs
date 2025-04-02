using MediatR;

namespace Dotnet.Template.Application.CQRS.Commands.Authentication;

public record RefreshTokenCommand(string RefreshToken) : IRequest<RefreshTokenResult>;

public record RefreshTokenResult(string AccessToken, string RefreshToken);
