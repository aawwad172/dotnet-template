using System.Reflection;
using System.Text;

using Dotnet.Template.Domain.Enums;
using Dotnet.Template.Presentation.API.Validators.Commands.Authentication;

using FluentValidation;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace Dotnet.Template.Presentation.API;

public static class DependencyInjection
{
    /// <summary>
    /// Registers Presentation layer services such as controllers, MediatR, FluentValidation, and any pipeline behaviors.
    /// </summary>
    /// <param name="services">The IServiceCollection to add services to.</param>
    /// <returns>The IServiceCollection for chaining.</returns>
    public static IServiceCollection AddPresentation(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register FluentValidation validators found in the current assembly.
        services.AddValidatorsFromAssemblyContaining<RegisterUserCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<LoginCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<RefreshTokenCommandValidator>();
        services.AddValidatorsFromAssemblyContaining<LogoutCommandValidator>();

        services.AddHttpContextAccessor();

        services.AddLogging(configure =>
        {
            configure.ClearProviders();
            configure.AddConsole();
            configure.AddDebug();
        });

        services.AddAuthorization(options =>
        {
            options.AddPolicy("SuperAdminPolicy", policy =>
                policy.RequireRole(RolesEnum.SuperAdmin.ToString()));

            options.AddPolicy("AdminPolicy", policy =>
                policy.RequireRole(RolesEnum.Admin.ToString()));

            options.AddPolicy("UserPolicy", policy =>
                policy.RequireRole(RolesEnum.User.ToString()));
        });

        // Optionally, register pipeline behaviors (for example, a transactional behavior).
        // services.AddTransient(typeof(IPipelineBehavior<,>), typeof(TransactionBehavior<,>));

        // Configure JWT authentication
        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = configuration["Jwt:Issuer"],
                ValidAudience = configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["Jwt:JwtSecretKey"]!))
            };
        });

        return services;
    }
}
