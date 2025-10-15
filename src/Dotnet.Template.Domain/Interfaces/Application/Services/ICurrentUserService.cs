namespace Dotnet.Template.Domain.Interfaces.Application.Services;

public interface ICurrentUserService
{
    Guid UserId { get; set; }
}
