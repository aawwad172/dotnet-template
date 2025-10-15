using Dotnet.Template.Domain.Entities;

namespace Dotnet.Template.Domain.Interfaces.Application.Services;

public interface IAuthenticationService
{
    Task<User> RegisterUserAsync(string firstName, string lastName, string email, string username, string password);
    Task<(string AccessToken, string RefreshToken)> LoginAsync(string email, string password);
    Task<(string AccessToken, string RefreshToken)> RefreshAccessTokenAsync(string refreshToken, Guid userId);
    Task LogoutAsync(string refreshToken, Guid userId);
}
