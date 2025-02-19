using System.Linq.Expressions;

using Dotnet.Template.Domain.Entities;
using Dotnet.Template.Domain.Exceptions;
using Dotnet.Template.Domain.Interfaces.IRepositories;
using Dotnet.Template.Infrastructure.Pagination;

using Microsoft.EntityFrameworkCore;

namespace Dotnet.Template.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
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
        var result = await _dbSet.AddAsync(entity);
        return result.Entity;
    }

    public async Task DeleteAsync(Ulid id)
    {
        T? entity = await _dbSet.FindAsync(id);
        if (entity is null)
            throw new NotFoundException($"The Record for Entity of type {typeof(T).Name} was not found.");

        _dbSet.Remove(entity);
    }

    public async Task<T> UpdateAsync(Ulid id)
    {
        T? entity = await _dbSet.FindAsync(id);
        if (entity is null)
            throw new NotFoundException($"The Record for Entity of type {typeof(T).Name} was not found.");

        var result = _dbSet.Update(entity);
        return result.Entity;
    }
}
