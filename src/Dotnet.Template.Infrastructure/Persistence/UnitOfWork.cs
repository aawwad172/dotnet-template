using Dotnet.Template.Domain.Interfaces.IRepositories;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Logging;

namespace Dotnet.Template.Infrastructure.Persistence;

public class UnitOfWork(BaseDbContext context, ILogger<UnitOfWork> logger) : IUnitOfWork
{
    private readonly BaseDbContext _context = context;
    private readonly ILogger<UnitOfWork> _logger = logger;
    private IDbContextTransaction? _transaction;

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        // If a transaction is already active, do nothing
        if (_transaction is null)
        {
            _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            _logger.LogInformation("Transaction started.");
        }
    }

    public async Task<int> SaveAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            int result = await _context.SaveChangesAsync(cancellationToken);
            _logger.LogInformation($"Changes saved to the database. {result} changes made.");
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while saving changes to the database.");
            throw;
        }
    }

    public async Task CommitAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.CommitAsync(cancellationToken);
            _logger.LogInformation("Transaction committed.");
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public async Task RollbackAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction is not null)
        {
            await _transaction.RollbackAsync(cancellationToken);
            _logger.LogInformation("Transaction rolled back.");
            _transaction.Dispose();
            _transaction = null;
        }
    }

    public void Detach<TEntity>(TEntity entity) where TEntity : class
    {
        _context.Entry(entity).State = EntityState.Detached;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
