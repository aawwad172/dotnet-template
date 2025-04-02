using MediatR;

namespace Dotnet.Template.Application.CQRS.Commands.Authentication;

public record LoginCommand(string Email, string Password) : IRequest<LoginResult>;

public record LoginResult(string AccessToken, string RefreshToken);
