using System.ComponentModel.DataAnnotations;
using System.Text.Json;

using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Presentation.API.Models;

namespace Dotnet.Template.Presentation.API.Middlewares;

public class ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
{
    private readonly RequestDelegate _next = next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger = logger;

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            _logger.LogWarning("NotFoundException occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, "NOT_FOUND", ex.Message, StatusCodes.Status404NotFound);
        }
        catch (EnvironmentVariableNotSetException ex)
        {
            _logger.LogWarning("EnvironmentVariableNotSetException occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, "ENV_VAR_MISSING", ex.Message, StatusCodes.Status500InternalServerError);
        }
        catch (ValidationException ex)
        {
            _logger.LogWarning("ValidationException occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, "VALIDATION_ERROR", ex.Message, StatusCodes.Status400BadRequest);
        }
        catch (UnauthenticatedException ex)
        {
            _logger.LogWarning("UnauthenticatedException occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, "UNAUTHENTICATED", ex.Message, StatusCodes.Status401Unauthorized);
        }
        catch (UnauthorizedException ex)
        {
            _logger.LogWarning("UnauthorizedException occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, "UNAUTHORIZED", ex.Message, StatusCodes.Status403Forbidden);
        }
        catch (ConflictException ex)
        {
            _logger.LogWarning("ConflictException occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, "CONFLICT", ex.Message, StatusCodes.Status409Conflict);
        }
        catch (CustomValidationException ex)
        {
            _logger.LogWarning("CustomValidationException occurred: {Message}", ex.Message);
            await HandleExceptionAsync(
                context,
                "VALIDATION_ERROR",
                JoinErrors(ex.Errors),
                StatusCodes.Status400BadRequest);
        }
        catch (Exception ex)
        {
            _logger.LogError("An unexpected error occurred: {Message}", ex.Message);
            await HandleExceptionAsync(context, "UNEXPECTED_ERROR", "An unexpected error occurred.", StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, string errorCode, string message, int statusCode)
    {
        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        ApiResponse<string> response = ApiResponse<string>.ErrorResponse(message, errorCode, statusCode);
        string result = JsonSerializer.Serialize(response);
        await context.Response.WriteAsync(result);
    }

    private string JoinErrors(IEnumerable<string> errors)
    {
        return string.Join(", ", errors);
    }
}
