using Dotnet.Template.Domain.Entities;

using Microsoft.EntityFrameworkCore;

namespace Dotnet.Template.Infrastructure.Pagination;

public static class QueryableExtensions
{
    // This method is an extension method that extends the IQueryable interface.
    // The default behavior of this method is to return everything from the query.
    public static async Task<PaginationResult<T>> ToPagedQueryAsync<T>(this IQueryable<T> query, int? pageNumber, int? pageSize)
    {
        int totalRecords = await query.CountAsync();

        if (pageSize is not null)
            query = query.Take((int)pageSize);

        if (pageNumber is not null)
            query = query.Skip(pageSize ?? 0 * ((int)pageNumber - 1));

        List<T> result = await query.ToListAsync();

        return new PaginationResult<T>
        {
            Page = result,
            TotalRecords = totalRecords,
            TotalDisplayRecords = result.Count
        };
    }
}
