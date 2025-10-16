using System.Reflection;
using System.Text;

using Dotnet.Template.Application.Utilities;
using Dotnet.Template.Domain.Entities.Authentication;
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
                ValidIssuer = configuration.GetRequiredSetting("Jwt:Issuer"),
                ValidAudience = configuration.GetRequiredSetting("Jwt:Audience"),
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.GetRequiredSetting("Jwt:JwtSecretKey")))
            };
        });

        services.AddAuthorization(options =>
        {
            // 1. Policy for creating a post (e.g., for /posts endpoint)
            options.AddPolicy("PostApprove", policy =>
            {
                policy.RequireAuthenticatedUser();
                // Check for a Claim with type "Permission" and value "Can_Create_Post"
                policy.RequireClaim(CustomClaims.Permission, PermissionsEnum.PostApprove);
            });

            // 2. Policy for managing users (e.g., for /users/ endpoint)
            options.AddPolicy("UserRead", policy =>
            {
                policy.RequireAuthenticatedUser();
                policy.RequireClaim(CustomClaims.Permission, PermissionsEnum.UserRead);
            });
        });

        return services;
    }
}
