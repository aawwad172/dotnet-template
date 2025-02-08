using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

using ProgressPath.Application.DTOs;
using ProgressPath.Application.Interfaces;
using ProgressPath.Domain.Exceptions;
using ProgressPath.Infrastructure.Pagination;

namespace ProgressPath.Infrastructure.Persistence.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly BaseDbContext _context;
    private readonly DbSet<T> _dbSet;

    public Repository(BaseDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }

    public async Task<PaginationResult<T>> GetAllAsync(Expression<Func<T, bool>>? filter, int? pageNumber, int? pageSize)
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

    public async Task<PaginationResult<T>> FindAsync(Expression<Func<T, bool>> filter)
    {
        return await _dbSet.Where(filter).ToPagedQueryAsync(null, null);
    }

    public async Task AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
    }

    public async Task DeleteAsync(T entity)
    {
        T? row = await _dbSet.FindAsync(entity);
        if (entity is null)
            throw new NotFoundException($"The Record for Entity of type {typeof(T).Name} was not found.");

        _dbSet.Remove(entity);
    }

    public async Task UpdateAsync(T entity)
    {
        T? row = await _dbSet.FindAsync(entity);
        if (entity is null)
            throw new NotFoundException($"The Record for Entity of type {typeof(T).Name} was not found.");

        _dbSet.Update(entity);
    }
}
