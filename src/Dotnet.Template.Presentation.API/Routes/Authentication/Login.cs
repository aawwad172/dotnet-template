
using Dotnet.Template.Application.CQRS.Commands.Authentication;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Presentation.API.Interfaces;
using Dotnet.Template.Presentation.API.Models;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Template.Presentation.API.Routes.Authentication;

public class Login : ICommandRoute<LoginCommand>
{
    public static async Task<IResult> RegisterRoute(
        [FromBody] LoginCommand request,
        [FromServices] IMediator mediator,
        [FromServices] IValidator<LoginCommand> validator)
    {
        ValidationResult validationResult = await validator.ValidateAsync(request);

        if (!validationResult.IsValid)
        {
            List<string> errors = validationResult.Errors
                    .Select(e => e.ErrorMessage)
                    .ToList();

            // Throw a custom ValidationException that your middleware will catch
            throw new CustomValidationException("Validation failed", errors);
        }

        LoginResult response = await mediator.Send(request);
        return Results.Ok(
            ApiResponse<LoginResult>.SuccessResponse(response));
    }
}
