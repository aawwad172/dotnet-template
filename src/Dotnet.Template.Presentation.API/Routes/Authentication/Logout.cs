using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Presentation.API.Models;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

namespace Dotnet.Template.Presentation.API.Routes.Authentication;

public class Logout
{
    public static async Task<IResult> RegisterRoute(
        LogoutCommand command,
        IMediator mediator,
        IValidator<LogoutCommand> validator)
    {
        ValidationResult? validationResult = await validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

            // Throw a custom ValidationException that your middleware will catch
            throw new CustomValidationException("Validation failed", errors);
        }

        var response = await mediator.Send(command);

        return Results.Ok(ApiResponse<LogoutResult>.SuccessResponse(response));
    }
}
