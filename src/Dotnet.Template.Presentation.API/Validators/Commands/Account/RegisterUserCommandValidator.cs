using Dotnet.Template.Application.CQRS.Commands.Authentication;

using FluentValidation;

namespace Dotnet.Template.Presentation.API.Validators.Commands.Account;

public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
{
    public RegisterUserCommandValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .WithMessage("First name is required.");

        RuleFor(x => x.LastName)
            .NotEmpty()
            .WithMessage("Last name is required.");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress()
            .WithMessage("A valid email address is required.");

        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username is required.")
            .Must(username => username == username.ToLowerInvariant())
            .WithMessage("Username must be in lowercase.");

        RuleFor(x => x.Password)
            .NotEmpty()
            .NotEmpty()
            .MinimumLength(8)
            .WithMessage("Password must be at least 8 characters long.")
            .Matches(@"^(?=.*[a-z])")
            .WithMessage("Password must contain at least one lowercase letter.")
            .Matches(@"^(?=.*[A-Z])")
            .WithMessage("Password must contain at least one uppercase letter.")
            .Matches(@"^(?=.*\d)")
            .WithMessage("Password must contain at least one number.")
            .Matches(@"^(?=.*[^\da-zA-Z])")
            .WithMessage("Password must contain at least one special character.");
    }
}
