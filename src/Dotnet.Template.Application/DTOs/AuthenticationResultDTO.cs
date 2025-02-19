namespace Dotnet.Template.Application.DTOs;

public record AuthenticationResultDTO
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}

