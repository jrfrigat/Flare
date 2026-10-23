using System.Reflection;
using Flare.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web.Virtualization;

namespace Flare.Components.Tests;

/// <summary>
/// ProvideVirtualItems composes two windows: the page the pager is on and the window Virtualize asks
/// for inside it. The component scenarios drive the real ItemsProvider through Virtualize and the
/// pager; the direct calls cover the window starts a headless renderer cannot scroll to.
/// </summary>
public class DataGridVirtualProviderTests : FlareTestContext
{
    private sealed record Person(string Name, string Team);

    private static readonly List<Person> People = Enumerable.Range(1, 25)
        .Select(i => new Person($"P{i:00}", i <= 10 ? "A" : "B"))
        .ToList();

    private static RenderFragment Cols() => b =>
    {
        b.OpenComponent<FlareColumn<Person>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Person, object?>)(p => p.Name));
        b.AddAttribute(3, "Sortable", true);
        b.CloseComponent();
    };

    private static RenderFragment Grouping() => b =>
    {
        b.OpenComponent<DataGridGroup<Person>>(0);
        b.AddAttribute(1, "Key", "Team");
        b.AddAttribute(2, "Selector", (Func<Person, object?>)(p => p.Team));
        b.CloseComponent();
    };

    private static Func<DataGridRequest, Task<DataGridResult<Person>>> Provider(
        List<DataGridRequest> seen, IReadOnlyList<Person>? data = null)
    {
        var rows = data ?? People;
        return request =>
        {
            seen.Add(request);
            var start = request.StartIndex ?? request.Page * request.PageSize;
            var count = request.Count ?? request.PageSize;
            return Task.FromResult(new DataGridResult<Person>(rows.Skip(start).Take(count), rows.Count));
        };
    }

    private IRenderedComponent<FlareDataGrid<Person>> RenderGrid(
        Func<DataGridRequest, Task<DataGridResult<Person>>> provider,
        Action<ComponentParameterCollectionBuilder<FlareDataGrid<Person>>>? extra = null)
        => Render<FlareDataGrid<Person>>(p =>
        {
            p.Add(x => x.ItemsProvider, provider);
            p.Add(x => x.Virtual, true);
            p.Add(x => x.PageSize, 10);
            p.Add(x => x.Columns, Cols());
            extra?.Invoke(p);
        });

    private static async Task<ItemsProviderResult<IndexedRow<Person>>> AskVirtualAsync(
        IRenderedComponent<FlareDataGrid<Person>> cut, int start, int count)
    {
        // Virtualize only asks for a window from zero in a headless renderer: there is no scroll
        // container to move. The method itself is the contract under test here.
        var method = typeof(FlareDataGrid<Person>).GetMethod("ProvideVirtualItems",
            BindingFlags.Instance | BindingFlags.NonPublic)!;
        var pending = (ValueTask<ItemsProviderResult<IndexedRow<Person>>>)method
            .Invoke(cut.Instance, new object[] { new ItemsProviderRequest(start, count, CancellationToken.None) })!;
        return await pending;
    }

    [Fact]
    public async Task VirtualProvider_InitialRequestIsTheFirstWindowOfThePage()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(Provider(seen));

        await cut.InvokeAsync(() => Task.CompletedTask);

        var first = Assert.Single(seen);
        Assert.Equal(0, first.StartIndex);
        Assert.InRange(first.Count!.Value, 1, 10);
        Assert.Equal(0, first.Page);
        Assert.Empty(first.Sorts);
        Assert.Empty(first.FilterModel);
        Assert.Empty(first.GroupKeys);
        Assert.Null(first.QuickFilter);
        Assert.Null(first.FilterTree);
        Assert.Contains("P01", cut.Find("tbody").TextContent);
        Assert.Equal(3, cut.Instance.PageCount);
    }

    [Fact]
    public async Task VirtualProvider_NextPageRequestsFromItsOffsetAndShowsItsRows()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(Provider(seen));

        await cut.InvokeAsync(() => cut.Instance.GoToPageAsync(1));

        var body = cut.Find("tbody").TextContent;
        Assert.Contains("P11", body);
        Assert.Contains(seen, r => r.StartIndex == 10);
        Assert.DoesNotContain("P01", body);
    }

    [Fact]
    public async Task VirtualProvider_LastPageShowsOnlyWhatIsLeft()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(Provider(seen));

        await cut.InvokeAsync(() => cut.Instance.GoToPageAsync(2));

        Assert.Contains(seen, r => r.StartIndex == 20);
        Assert.Equal(5, cut.FindAll($"tr.{Css.Classes.DataGrid.Row}").Count);
        Assert.Contains("P25", cut.Find("tbody").TextContent);
    }

    [Fact]
    public async Task VirtualProvider_EmptyResponseLeavesNoRows()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(Provider(seen, Array.Empty<Person>()));

        await cut.InvokeAsync(() => Task.CompletedTask);

        Assert.Empty(cut.FindAll($"tr.{Css.Classes.DataGrid.Row}"));
        Assert.Single(seen);
    }

    [Fact]
    public async Task VirtualProvider_SortReachesTheRequest()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(Provider(seen));

        await cut.InvokeAsync(() => cut.FindAll("thead th")[0].Click());

        var sorted = Assert.Single(seen, r => r.Sorts.Count > 0);
        Assert.Equal(SortDirection.Ascending, sorted.Sorts[0].Direction);
        Assert.NotNull(sorted.SortKey);
    }

    [Fact]
    public async Task VirtualProvider_QuickFilterReachesTheRequest()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(Provider(seen));

        await cut.InvokeAsync(() => cut.Instance.ApplyQuickFilter("P1"));

        Assert.Contains(seen, r => r.QuickFilter == "P1");
    }

    [Fact]
    public async Task VirtualProvider_AdvancedFilterReachesTheRequest()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(Provider(seen));

        var tree = new DataGridFilterGroup(false,
            new[] { new DataGridFilter("Name", FilterOperator.Contains, "P1") }, []);
        await cut.InvokeAsync(() => cut.Instance.ApplyAdvancedFilter(tree));

        var filtered = Assert.Single(seen, r => r.FilterTree is not null);
        Assert.Single(filtered.FilterTree!.Conditions);
        Assert.False(filtered.FilterTree.Or);
    }

    [Fact]
    public async Task VirtualProvider_CarriesGroupKeysIntoTheRequest()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(Provider(seen), p => p.Add(x => x.Grouping, Grouping()));

        // A grouped grid shapes its lines from the loaded page, so Virtualize does not ask the
        // provider for rows; the mapping still has to carry the keys a server would group by.
        await AskVirtualAsync(cut, start: 0, count: 10);

        Assert.Equal(new[] { "Team" }, seen[^1].GroupKeys);
    }

    [Fact]
    public async Task WindowInsideAPage_AddsThePageOffset()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(Provider(seen));
        await cut.InvokeAsync(() => cut.Instance.GoToPageAsync(1));
        seen.Clear();

        var result = await AskVirtualAsync(cut, start: 3, count: 7);

        var request = seen[^1];
        Assert.Equal(13, request.StartIndex);
        Assert.Equal(7, request.Count);
        Assert.Equal(7, result.Items.Count());
        Assert.Equal(3, result.Items.First().Index);
        Assert.Equal("P14", result.Items.First().Item.Name);
    }

    [Fact]
    public async Task WindowPastThePageEnd_ClipsTheCountAndTheScrollableTotal()
    {
        var seen = new List<DataGridRequest>();
        var cut = RenderGrid(Provider(seen));
        await cut.InvokeAsync(() => cut.Instance.GoToPageAsync(2));
        seen.Clear();

        var result = await AskVirtualAsync(cut, start: 6, count: 10);

        var request = seen[^1];
        Assert.Equal(26, request.StartIndex);
        Assert.Equal(4, request.Count);
        Assert.Equal(5, result.TotalItemCount);
    }

    [Fact]
    public async Task NoProvider_ReturnsAnEmptyWindow()
    {
        var cut = Render<FlareDataGrid<Person>>(p => p
            .Add(x => x.Items, People)
            .Add(x => x.Columns, Cols()));

        var result = await AskVirtualAsync(cut, start: 0, count: 10);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalItemCount);
    }
}
