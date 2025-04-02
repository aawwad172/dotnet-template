using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Presentation.API.Models;

using FluentValidation;

using MediatR;

namespace Dotnet.Template.Presentation.API.Routes.Account;

public class RegisterUser
{
    public static async Task<IResult> RegisterRoute(
           RegisterUserCommand command,
           IMediator mediator,
           IValidator<RegisterUserCommand> validator)
    {
        var validationResult = await validator.ValidateAsync(command);

        if (!validationResult.IsValid)
        {
            var errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

            // Throw a custom ValidationException that your middleware will catch
            throw new CustomValidationException("Validation failed", errors);
        }

        RegisterUserResult response = await mediator.Send(command);
        return Results.Created(
            $"/users/{response.user.Id}",
            ApiResponse<RegisterUserResult>.SuccessResponse(response));
    }
}
