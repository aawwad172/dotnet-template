using System.Linq.Expressions;

using ProgressPath.Application.DTOs;

namespace ProgressPath.Application.Interfaces;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Ulid id);
    Task<PaginationResult<T>> GetAllAsync(Expression<Func<T, bool>>? filter, int? pageNumber, int? pageSize);
    Task<PaginationResult<T>> FindAsync(Expression<Func<T, bool>> filter);
    Task AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(T entity);
}
