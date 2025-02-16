using System.Security.Claims;

using Dotnet.Template.Application.Interfaces;

namespace Dotnet.Template.Presentation.API.Middlewares;

/// <summary>
/// Middleware for validating JWT tokens and attaching user information to the HTTP context.
/// </summary>
/// <remarks>
/// Make sure to update the configuration settings for "Jwt:JwtSecretKey", "Jwt:JwtIssuer", and "Jwt:JwtAudience" as needed.
/// </remarks>
public class JwtMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IJwtService _jwtService;

    public JwtMiddleware(RequestDelegate next, IJwtService jwtService)
    {
        _next = next;
        _jwtService = jwtService;
    }

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
            ClaimsPrincipal principal = _jwtService.ValidateToken(token);
            if (principal != null)
            {
                context.User = principal;
            }
        }

        await _next(context);
    }
}
