using System.Linq.Expressions;

using Dotnet.Template.Domain.Entities;

namespace Dotnet.Template.Domain.Interfaces.IRepositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Guid id);
    Task<PaginationResult<T>> GetAllAsync(
        int? pageNumber,
        int? pageSize,
        Expression<Func<T, bool>>? filter);
    Task<T> AddAsync(T entity);
    Task<T?> UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
