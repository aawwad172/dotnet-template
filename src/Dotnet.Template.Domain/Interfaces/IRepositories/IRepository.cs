using System.Linq.Expressions;

using Dotnet.Template.Domain.Entities;

namespace Dotnet.Template.Domain.Interfaces.IRepositories;

public interface IRepository<T> where T : class
{
    Task<T?> GetByIdAsync(Ulid id);
    Task<PaginationResult<T>> GetAllAsync(
        int? pageNumber,
        int? pageSize,
        Expression<Func<T, bool>>? filter);
    Task AddAsync(T entity);
    Task UpdateAsync(Ulid id);
    Task DeleteAsync(Ulid id);
}
