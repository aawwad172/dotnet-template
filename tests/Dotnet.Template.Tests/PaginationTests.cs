using Dotnet.Template.Infrastructure.Pagination;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Dotnet.Template.Tests;

public class PaginationTests
{
    private class Item
    {
        public int Id { get; set; }
    }

    private class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
    {
        public DbSet<Item> Items => Set<Item>();
    }

    private static async Task<TestDbContext> CreateContextAsync(IEnumerable<Item> seed)
    {
        var options = new DbContextOptionsBuilder<TestDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var context = new TestDbContext(options);
        context.Items.AddRange(seed);
        await context.SaveChangesAsync();
        return context;
    }

    [Fact]
    public async Task Returns_first_page_when_page_number_and_size_provided()
    {
        var items = Enumerable.Range(1, 20).Select(i => new Item { Id = i });
        await using var context = await CreateContextAsync(items);

        var result = await context.Items.OrderBy(i => i.Id).AsQueryable()
            .ToPagedQueryAsync(1, 5);

        Assert.Equal(20, result.TotalRecords);
        Assert.Equal(5, result.TotalDisplayRecords);
        Assert.Equal(new[] { 1, 2, 3, 4, 5 }, result.Page!.Select(i => i.Id));
    }

    [Fact]
    public async Task Returns_second_page_based_on_page_size()
    {
        var items = Enumerable.Range(1, 12).Select(i => new Item { Id = i });
        await using var context = await CreateContextAsync(items);

        var result = await context.Items.OrderBy(i => i.Id).AsQueryable()
            .ToPagedQueryAsync(2, 5);

        Assert.Equal(12, result.TotalRecords);
        Assert.Equal(5, result.TotalDisplayRecords);
        Assert.Equal(new[] { 6, 7, 8, 9, 10 }, result.Page!.Select(i => i.Id));
    }

    [Fact]
    public async Task Handles_null_page_size_by_returning_all_items()
    {
        var items = Enumerable.Range(1, 3).Select(i => new Item { Id = i });
        await using var context = await CreateContextAsync(items);

        var result = await context.Items.OrderBy(i => i.Id).AsQueryable()
            .ToPagedQueryAsync(1, null);

        Assert.Equal(3, result.TotalRecords);
        Assert.Equal(3, result.TotalDisplayRecords);
        Assert.Equal(new[] { 1, 2, 3 }, result.Page!.Select(i => i.Id));
    }

    [Fact]
    public async Task Handles_null_page_number_without_skipping()
    {
        var items = Enumerable.Range(1, 4).Select(i => new Item { Id = i });
        await using var context = await CreateContextAsync(items);

        var result = await context.Items.OrderBy(i => i.Id).AsQueryable()
            .ToPagedQueryAsync(null, 2);

        Assert.Equal(4, result.TotalRecords);
        Assert.Equal(2, result.TotalDisplayRecords);
        Assert.Equal(new[] { 1, 2 }, result.Page!.Select(i => i.Id));
    }

    [Fact]
    public async Task Returns_empty_page_when_page_exceeds_data()
    {
        var items = Enumerable.Range(1, 5).Select(i => new Item { Id = i });
        await using var context = await CreateContextAsync(items);

        var result = await context.Items.OrderBy(i => i.Id).AsQueryable()
            .ToPagedQueryAsync(3, 5);

        Assert.Equal(5, result.TotalRecords);
        Assert.Empty(result.Page!);
        Assert.Equal(0, result.TotalDisplayRecords);
    }
}
