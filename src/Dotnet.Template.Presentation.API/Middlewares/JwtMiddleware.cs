using System.Security.Claims;

using Dotnet.Template.Application.Services;
using Dotnet.Template.Domain.Interfaces.Application.Services;

namespace Dotnet.Template.Presentation.API.Middlewares;

/// <summary>
/// Middleware for validating JWT tokens and attaching user information to the HTTP context.
/// </summary>
/// <remarks>
/// Make sure to update the configuration settings for "Jwt:JwtSecretKey", "Jwt:JwtIssuer", and "Jwt:JwtAudience" as needed.
/// Visit https://jwtsecret.com/generate for generating a secure JWT secret key.
/// </remarks>
public class JwtMiddleware(
    RequestDelegate next,
    ILogger<JwtMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<JwtMiddleware> _logger = logger;

    /// <summary>
    /// Invokes the middleware to validate the JWT token from the request header and attach the user to the context.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context, ICurrentUserService currentUser)
    {
        IJwtService _jwtService = context.RequestServices.GetRequiredService<IJwtService>();

        string? token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                ClaimsPrincipal? principal = await _jwtService.ValidateToken(token);
                if (principal is not null)
                {
                    context.User = principal;

                    // set the current user service properties (safe because CurrentUserService is scoped)
                    // try common claim types: NameIdentifier (ClaimTypes.NameIdentifier), "sub", "id"
                    var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value
                                 ?? principal.FindFirst("sub")?.Value
                                 ?? principal.FindFirst("id")?.Value;

                    if (!string.IsNullOrEmpty(userId))
                    {
                        currentUser.UserId = Guid.Parse(userId);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to validate JWT token");
            }
        }
        else
        {
            // Optionally: if another authentication middleware populated context.User
            // populate currentUser from context.User (useful if you call UseAuthentication earlier)
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                var userId = context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                             ?? context.User.FindFirst("sub")?.Value
                             ?? context.User.FindFirst("id")?.Value;
                if (!string.IsNullOrEmpty(userId) && currentUser is CurrentUserService impl)
                    impl.UserId = Guid.Parse(userId);
            }
        }

        await _next(context);
    }
}
