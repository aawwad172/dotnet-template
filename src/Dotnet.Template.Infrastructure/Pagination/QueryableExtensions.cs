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

        if (pageNumber is not null && pageSize is not null)
        {
            int skip = pageSize.Value * (pageNumber.Value - 1);
            query = query.Skip(skip).Take(pageSize.Value);
        }
        else if (pageSize is not null)
        {
            query = query.Take(pageSize.Value);
        }

        List<T> result = await query.ToListAsync();

        return new PaginationResult<T>
        {
            Page = result,
            TotalRecords = totalRecords,
            TotalDisplayRecords = result.Count
        };
    }
}
