using Dotnet.Template.Application.CQRS.Commands.Authentication;

using FluentValidation;

namespace Dotnet.Template.Presentation.API.Validators.Commands.Authentication;

public class LoginCommandValidator : AbstractValidator<LoginCommand>
{
    public LoginCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password is required.");
    }
}
