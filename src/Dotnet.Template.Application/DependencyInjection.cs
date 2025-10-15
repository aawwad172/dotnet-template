using Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;
using Dotnet.Template.Application.Services;
using Dotnet.Template.Domain.Interfaces.Application.Services;

using MapsterMapper;

using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Template.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommandHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(LoginCommandHandler).Assembly);
            cfg.RegisterServicesFromAssembly(typeof(LogoutCommandHandler).Assembly);
        });
        services.AddScoped<ISecurityService, SecurityService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IPermissionService, PermissionService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IMapper, Mapper>();

        return services;
    }
}
