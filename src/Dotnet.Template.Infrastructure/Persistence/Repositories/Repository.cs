using System.Linq.Expressions;

using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Interfaces;
using Dotnet.Template.Domain.Interfaces.IRepositories;
using Dotnet.Template.Infrastructure.Pagination;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Dotnet.Template.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly BaseDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(BaseDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<PaginationResult<T>> GetAllAsync(
        int? pageNumber = null,
        int? pageSize = null,
        Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _dbSet;
        if (filter is not null)
            query = query.Where(filter!);

        return await query.ToPagedQueryAsync(pageNumber, pageSize);
    }

    public async Task<T?> GetByIdAsync(Ulid id)
    {
        return await _dbSet.FindAsync(id);
    }

    public async Task<T> AddAsync(T entity)
    {
        EntityEntry<T> result = await _dbSet.AddAsync(entity);
        return result.Entity;
    }

    /// <summary>
    /// Deletes an entity by its id.
    /// If the entity is not found, nothing happens.
    /// The service layer can decide how to handle a "not found" case.
    /// </summary>
    public async Task DeleteAsync(Ulid id)
    {
        T? entity = await _dbSet.FindAsync(id);
        if (entity is not null)
            _dbSet.Remove(entity);

        // If entity is null, we simply do nothing.
    }

    /// <summary>
    /// Updates an entity.
    /// If the entity is not found, returns null so the service can handle it.
    /// </summary>
    public async Task<T?> UpdateAsync(T entity)
    {
        // Attempt to find the entity in the database
        T? existingEntity = await _dbSet.FindAsync(entity.Id);
        if (existingEntity is null)
            // Return null instead of throwing an exception.
            return null;

        // Otherwise, update the entity.
        EntityEntry<T> result = _dbSet.Update(entity);
        return result.Entity;
    }
}
