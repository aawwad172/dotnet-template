using Dotnet.Template.Application.CQRS.CommandHandlers.Authentication;
using Dotnet.Template.Application.HelperServices;
using Dotnet.Template.Application.Interfaces.Services;

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
        });
        services.AddScoped<IEncryptionService, EncryptionService>();
        services.AddScoped<IAuthenticationService, AuthenticationService>();
        services.AddScoped<IMapper, Mapper>();

        return services;
    }
}
