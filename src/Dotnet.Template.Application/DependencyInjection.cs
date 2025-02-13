using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Template.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            // cfg.RegisterServicesFromAssembly(typeof(Your_Query_Handler_Here).Assembly);
        });

        return services;
    }
}
