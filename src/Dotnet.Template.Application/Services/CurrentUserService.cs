using Dotnet.Template.Domain.Interfaces.Application.Services;

namespace Dotnet.Template.Application.Services;

public class CurrentUserService : ICurrentUserService
{
    public Guid UserId { get; set; }
}
