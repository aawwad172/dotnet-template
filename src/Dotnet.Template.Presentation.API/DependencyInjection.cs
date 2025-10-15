using System.Reflection;
using System.Text;

using Dotnet.Template.Application.Utilities;
using Dotnet.Template.Domain.Entities.Authentication;
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
            // ------------------------------------------------------------------
            // A. Role-Based Policies (For high-level access control)
            // ------------------------------------------------------------------

            // 1. User Policy (Accessible by anyone who is a user)
            // NOTE: If you have different user levels (Admin/SuperAdmin) that should also count as 'User',
            // you must explicitly list them.
            options.AddPolicy(PolicyNames.UserOnly, policy =>
                policy.RequireRole("User", "Admin", "SuperAdmin", "SubAdmin"));

            // 2. Admin or Above Policy (Standard Admin level access)
            options.AddPolicy(PolicyNames.AdminOrAbove, policy =>
                policy.RequireRole("Admin", "SuperAdmin"));

            // 3. Super Admin Only Policy (Highest access level)
            options.AddPolicy(PolicyNames.SuperAdminOnly, policy =>
                policy.RequireRole("SuperAdmin"));

            // ------------------------------------------------------------------
            // B. Permission-Based Policies (Managed by Custom Handler)
            // ------------------------------------------------------------------

            // NOTE: You don't register individual permission policies here.
            // They are handled dynamically by your custom IAuthorizationPolicyProvider, 
            // which looks for the "Permission:" prefix, thus keeping this section clean.

            /* Example of a dynamic policy (Do NOT un-comment, this is handled by your custom code):
            options.AddPolicy("Permission:User.Create", policy =>
                policy.Requirements.Add(new PermissionRequirement("User.Create")));
            */
        });

        return services;
    }
}
