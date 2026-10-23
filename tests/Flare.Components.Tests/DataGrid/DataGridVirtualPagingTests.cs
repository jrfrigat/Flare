using Flare.Components;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// TASK-695: in virtual + provider mode the rows are rendered by the provider Virtualize, so a page
/// change or a quick filter has to refresh that window instead of only loading a cache nothing shows.
/// </summary>
public class DataGridVirtualPagingTests : FlareTestContext
{
    private sealed record Person(string Name);

    private static readonly List<Person> People = Enumerable.Range(1, 25)
        .Select(i => new Person($"P{i:00}"))
        .ToList();

    private static RenderFragment Cols() => b =>
    {
        b.OpenComponent<FlareColumn<Person>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Person, object?>)(p => p.Name));
        b.CloseComponent();
    };

    private static Func<DataGridRequest, Task<DataGridResult<Person>>> Provider(List<DataGridRequest> seen)
        => request =>
        {
            seen.Add(request);
            IEnumerable<Person> rows = People;
            if (!string.IsNullOrEmpty(request.QuickFilter))
                rows = rows.Where(p => p.Name.Contains(request.QuickFilter, StringComparison.Ordinal));
            var matches = rows.ToList();
            var start = request.StartIndex ?? request.Page * request.PageSize;
            var count = request.Count ?? request.PageSize;
            return Task.FromResult(new DataGridResult<Person>(matches.Skip(start).Take(count), matches.Count));
        };

    private IRenderedComponent<FlareDataGrid<Person>> RenderGrid(List<DataGridRequest> seen)
        => Render<FlareDataGrid<Person>>(p => p
            .Add(x => x.ItemsProvider, Provider(seen))
            .Add(x => x.Virtual, true)
            .Add(x => x.PageSize, 10)
            .Add(x => x.Columns, Cols()));

    [Fact]
    public async Task NextPage_RefreshesTheVirtualWindow()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(seen);
        Assert.Contains("P01", cut.Find("tbody").TextContent);

        await cut.InvokeAsync(() => cut.Instance.GoToPageAsync(1));

        Assert.Contains(seen, r => r.StartIndex == 10);
        var body = cut.Find("tbody").TextContent;
        Assert.Contains("P11", body);
        Assert.DoesNotContain("P01", body);
    }

    [Fact]
    public async Task QuickFilter_RefreshesTheVirtualWindow()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(seen);

        await cut.InvokeAsync(() => cut.Instance.ApplyQuickFilter("P2"));

        var body = cut.Find("tbody").TextContent;
        Assert.Contains("P20", body);
        Assert.DoesNotContain("P01", body);
        Assert.Contains(seen, r => r.QuickFilter == "P2");
    }
}
