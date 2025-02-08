using Microsoft.Extensions.DependencyInjection;

namespace ProgressPath.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
