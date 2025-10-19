using Dotnet.Template.Domain.Interfaces.Application.Services;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Dotnet.Template.Application.CQRS;

public abstract class BaseHandler<TRequest, TResponse> : IRequestHandler<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    protected readonly ICurrentUserService _currentUser;
    protected readonly ILogger _logger;
    protected readonly IUnitOfWork _unitOfWork;

    protected BaseHandler(ICurrentUserService currentUserService, ILogger logger, IUnitOfWork unitOfWork)
    {
        _currentUser = currentUserService ?? throw new ArgumentNullException(nameof(currentUserService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
    }

    // Make this abstract to force derived handlers implement it,
    // or virtual if you want a default behavior.
    public abstract Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken);
}
