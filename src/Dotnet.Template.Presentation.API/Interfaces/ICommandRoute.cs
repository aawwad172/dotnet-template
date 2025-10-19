using FluentValidation;

using MediatR;

using Microsoft.AspNetCore.Mvc;

namespace Dotnet.Template.Presentation.API.Interfaces;

public interface ICommandRoute<TRequest> where TRequest : notnull
{
    static abstract Task<IResult> RegisterRoute(
        [FromBody] TRequest request,
        [FromServices] IMediator mediator,
        [FromServices] IValidator<TRequest> validator);
}
