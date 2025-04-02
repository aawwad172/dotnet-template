using MediatR;

namespace Dotnet.Template.Application.CQRS.Commands.Authentication;

public record LoginCommand(string email, string password) : IRequest<LoginResult>;

public record LoginResult(string AccessToken, string RefreshToken);
