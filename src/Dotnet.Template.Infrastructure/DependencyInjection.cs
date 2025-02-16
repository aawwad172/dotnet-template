using Dotnet.Template.Domain.Interfaces.IRepositories;
using Dotnet.Template.Infrastructure.Persistence;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Template.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services)
    {
        string connectionString = DbConfig.GetDBConnectionString()!;

        services.AddDbContext<BaseDbContext>((provider, options) => options.UseNpgsql(connectionString));
        // Add your repositories like this here
        // services.AddScoped<IRepository, Repository>();

        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddLogging();

        return services;
    }
}
