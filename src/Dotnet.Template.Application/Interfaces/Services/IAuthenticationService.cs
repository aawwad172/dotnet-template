namespace Dotnet.Template.Application.Interfaces.Services;

public interface IAuthenticationService
{
    Task<(string AccessToken, string RefreshToken)> LoginAsync(string email, string password);
    Task<(string AccessToken, string RefreshToken)> RefreshAccessTokenAsync(string refreshToken);
    Task LogoutAsync(string refreshToken);
}
