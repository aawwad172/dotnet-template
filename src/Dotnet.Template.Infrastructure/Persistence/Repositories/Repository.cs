using System.Linq.Expressions;

using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Interfaces.Domain;
using Dotnet.Template.Domain.Interfaces.Infrastructure.IRepositories;
using Dotnet.Template.Infrastructure.Pagination;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace Dotnet.Template.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class, IEntity
{
    private readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public virtual async Task<PaginationResult<T>> GetAllAsync(
        int? pageNumber = null,
        int? pageSize = null,
        Expression<Func<T, bool>>? filter = null)
    {
        IQueryable<T> query = _dbSet;
        if (filter is not null)
            query = query.Where(filter!);

        return await query.ToPagedQueryAsync(pageNumber, pageSize);
    }

    public virtual async Task<T?> GetByIdAsync(Guid id)
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
    public async Task DeleteAsync(Guid id)
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
    public T? Update(T entity)
    {
        // We assume the entity has either been fetched and is tracked, OR it is detached.
        // We only explicitly update the entity if it is NOT currently tracked.

        EntityEntry<T> entry = _context.Entry(entity);

        if (entry.State == EntityState.Detached)
        {
            // If detached, attach the entity and mark it as modified.
            // This is the cleanest way to update an entity that came from outside the context.
            _dbSet.Attach(entity);
            entry.State = EntityState.Modified;
        }
        // If the entity is already tracked (like in your login handler), its state is already
        // marked as 'Modified' or 'Unchanged'. We just let the change tracker handle it.

        // Return the entity for fluent API chaining if needed.
        return entity;
    }
}
