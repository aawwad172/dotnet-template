using Dotnet.Template.Application.CQRS.Commands.Authentication;

using FluentValidation;

namespace Dotnet.Template.Presentation.API.Validators.Commands.Authentication;

public class LogoutCommandValidator : AbstractValidator<LogoutCommand>
{
    public LogoutCommandValidator()
    {
        RuleFor(x => x.RefreshToken)
            .NotEmpty()
            .WithMessage("Refresh token is required.");
    }
}
