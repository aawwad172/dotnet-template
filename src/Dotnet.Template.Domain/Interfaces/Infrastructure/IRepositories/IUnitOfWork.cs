namespace Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;

public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    Task<int> SaveAsync(CancellationToken cancellationToken = default);

    Task CommitAsync(CancellationToken cancellationToken = default);

    Task RollbackAsync(CancellationToken cancellationToken = default);

    void Detach<TEntity>(TEntity entity) where TEntity : class;
}
