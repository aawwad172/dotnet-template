using Microsoft.Extensions.DependencyInjection;

namespace ProgressPath.Domain;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        return services;
    }
}
