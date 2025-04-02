using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Presentation.API.Models;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

namespace Dotnet.Template.Presentation.API.Routes.Authentication;

public class Login
{
    public static async Task<IResult> RegisterRoute(
               LoginCommand command,
               IMediator mediator,
               IValidator<LoginCommand> validator)
    {
        ValidationResult validationResult = await validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

            // Throw a custom ValidationException that your middleware will catch
            throw new CustomValidationException("Validation failed", errors);
        }

        LoginResult response = await mediator.Send(command);
        return Results.Ok(
            ApiResponse<LoginResult>.SuccessResponse(response));
    }
}
