using System.Security.Claims;

using Dotnet.Template.Application.Interfaces.Services;

namespace Dotnet.Template.Presentation.API.Middlewares;

/// <summary>
/// Middleware for validating JWT tokens and attaching user information to the HTTP context.
/// </summary>
/// <remarks>
/// Make sure to update the configuration settings for "Jwt:JwtSecretKey", "Jwt:JwtIssuer", and "Jwt:JwtAudience" as needed.
/// </remarks>
public class JwtMiddleware(
    RequestDelegate next,
    IJwtService jwtService,
    ILogger<JwtMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly IJwtService _jwtService = jwtService;
    private readonly ILogger<JwtMiddleware> _logger = logger;

    /// <summary>
    /// Invokes the middleware to validate the JWT token from the request header and attach the user to the context.
    /// </summary>
    /// <param name="context">The current HTTP context.</param>
    /// <returns>A <see cref="Task"/> that represents the asynchronous operation.</returns>
    public async Task Invoke(HttpContext context)
    {
        string? token = context.Request.Headers["Authorization"]
            .FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            try
            {
                Task<ClaimsPrincipal>? result = _jwtService.ValidateToken(token);
                if (result is not null)
                    context.User = await result;
            }
            catch (Exception ex)
            {
                // Log the error but don't throw to allow the request to continue
                _logger.LogError(ex, "Failed to validate JWT token");
            }
        }

        await _next(context);
    }
}
