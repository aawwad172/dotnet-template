using Dotnet.Template.Domain.Interfaces.Application.Services;

using MediatR;

namespace Dotnet.Template.Application.CQRS;

public abstract class BaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected readonly ICurrentUserService _currentUser;

    protected BaseHandler(ICurrentUserService currentUserService)
    {
        _currentUser = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
    }

    // Make this abstract to force derived handlers implement it,
    // or virtual if you want a default behavior.
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
