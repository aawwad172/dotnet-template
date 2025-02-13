namespace Dotnet.Template.Domain.Interfaces.IRepositories;

public interface IUnitOfWork : IDisposable
{
    Task BeginTransactionAsync(CancellationToken cancellationToken);

    Task<int> SaveAsync(CancellationToken cancellationToken);

    Task CommitAsync(CancellationToken cancellationToken);

    Task RollbackAsync(CancellationToken cancellationToken);

    void Detach<TEntity>(TEntity entity) where TEntity : class;
}
