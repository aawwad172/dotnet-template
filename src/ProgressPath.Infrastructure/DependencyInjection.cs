using Microsoft.Extensions.DependencyInjection;

namespace ProgressPath.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
