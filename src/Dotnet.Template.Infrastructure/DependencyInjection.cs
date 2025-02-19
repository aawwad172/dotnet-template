using Dotnet.Template.Application.Utilities;
using Dotnet.Template.Domain.Interfaces.IRepositories;
using Dotnet.Template.Infrastructure.Persistence;
using Dotnet.Template.Infrastructure.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Dotnet.Template.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetRequiredSetting("ConnectionStrings:DbConnectionString");

        services.AddDbContext<BaseDbContext>((IServiceProvider provider, DbContextOptionsBuilder options) => options.UseNpgsql(connectionString));
        // Add your repositories like this here
        // services.AddScoped<IRepository, Repository>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddLogging();

        return services;
    }
}
