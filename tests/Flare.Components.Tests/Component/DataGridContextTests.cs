using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests.Component;

/// <summary>
/// The external control surface: a DataGridContext declared by the page must be able to read, drive and
/// observe a grid it is not rendered inside. These tests stand in for the third-party control that has
/// no access to grid internals - everything here goes through the public context only.
/// </summary>
public class DataGridContextTests : FlareTestContext
{
    private record Row(string Name, int Qty);

    private static readonly Row[] _rows =
    [
        new("Beta", 2),
        new("Alpha", 1),
        new("Gamma", 3),
        new("Delta", 4),
    ];

    private static RenderFragment Grid(DataGridContext<Row>? ctx, int pageSize = 10) => b =>
    {
        b.OpenComponent<FlareDataGrid<Row>>(0);
        b.AddAttribute(1, "Items", _rows.AsEnumerable());
        b.AddAttribute(2, "PageSize", pageSize);
        if (ctx is not null) b.AddAttribute(3, "Context", ctx);
        b.AddAttribute(4, "Columns", (RenderFragment)(inner =>
        {
            inner.OpenComponent<FlareColumn<Row>>(10);
            inner.AddAttribute(11, "Title", "Name");
            inner.AddAttribute(12, "Field", (Func<Row, object?>)(r => r.Name));
            inner.AddAttribute(13, "Sortable", true);
            inner.AddAttribute(14, "Filterable", true);
            inner.CloseComponent();

            inner.OpenComponent<FlareColumn<Row>>(20);
            inner.AddAttribute(21, "Title", "Qty");
            inner.AddAttribute(22, "Field", (Func<Row, object?>)(r => r.Qty));
            inner.AddAttribute(23, "Sortable", true);
            inner.CloseComponent();
        }));
        b.CloseComponent();
    };

    [Fact]
    public void Supplied_context_attaches_to_the_grid()
    {
        var ctx = new DataGridContext<Row>();
        Assert.False(ctx.IsAttached);

        Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx)));

        Assert.True(ctx.IsAttached);
        Assert.Equal(["Name", "Qty"], ctx.Columns.Select(c => c.Title));
    }

    [Fact]
    public void Grid_without_a_supplied_context_still_has_one()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, _rows)
            .Add(x => x.Columns, (RenderFragment)(b => { })));

        Assert.NotNull(cut.Instance.ActiveContext);
        Assert.True(cut.Instance.ActiveContext.IsAttached);
    }

    [Fact]
    public void Sorting_through_the_context_reorders_the_rendered_rows()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx)));

        cut.InvokeAsync(() => ctx.SortByAsync("Name")).GetAwaiter().GetResult();

        Assert.Equal(SortDirection.Ascending, Assert.Single(ctx.Sorts).Direction);
        var firstCell = cut.FindAll("tbody td")[0].TextContent.Trim();
        Assert.Equal("Alpha", firstCell);
    }

    [Fact]
    public void SetSorts_replaces_the_whole_stack_and_drops_unknown_keys()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx)));

        cut.InvokeAsync(() => ctx.SetSortsAsync(
        [
            new DataGridSort("Qty", SortDirection.Descending),
            new DataGridSort("NotAColumn", SortDirection.Ascending),
        ])).GetAwaiter().GetResult();

        var sort = Assert.Single(ctx.Sorts);
        Assert.Equal("Qty", sort.Key);
        Assert.Equal(SortDirection.Descending, sort.Direction);

        cut.InvokeAsync(() => ctx.ClearSortsAsync()).GetAwaiter().GetResult();
        Assert.Empty(ctx.Sorts);
    }

    [Fact]
    public void Quick_filter_through_the_context_narrows_the_rows()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx)));
        Assert.Equal(4, cut.FindAll("tbody tr").Count);

        cut.InvokeAsync(() => ctx.SetQuickFilterAsync("elt")).GetAwaiter().GetResult();

        Assert.Equal("elt", ctx.QuickFilterText);
        Assert.Equal(1, ctx.FilteredCount);
        Assert.Single(cut.FindAll("tbody tr"));

        cut.InvokeAsync(() => ctx.ClearFiltersAsync()).GetAwaiter().GetResult();
        Assert.Null(ctx.QuickFilterText);
        Assert.Equal(4, cut.FindAll("tbody tr").Count);
    }

    [Fact]
    public void Column_visibility_and_order_are_drivable_from_outside()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx)));

        cut.InvokeAsync(() => ctx.SetColumnVisibleAsync("Qty", false)).GetAwaiter().GetResult();
        Assert.Equal(["Name"], ctx.VisibleColumns.Select(c => c.Title));
        Assert.Contains("Qty", ctx.HiddenColumnKeys);

        cut.InvokeAsync(() => ctx.SetColumnVisibleAsync("Qty", true)).GetAwaiter().GetResult();
        Assert.Equal(["Name", "Qty"], ctx.VisibleColumns.Select(c => c.Title));

        cut.InvokeAsync(() => ctx.MoveColumnAsync("Qty", "Name")).GetAwaiter().GetResult();
        Assert.Equal(["Qty", "Name"], ctx.ColumnOrder);
    }

    [Fact]
    public void Setting_a_column_visible_when_it_already_is_changes_nothing()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx)));
        var changes = 0;
        ctx.Changed += _ => changes++;

        cut.InvokeAsync(() => ctx.SetColumnVisibleAsync("Qty", true)).GetAwaiter().GetResult();

        Assert.Equal(0, changes);
    }

    [Fact]
    public void Paging_through_the_context_moves_the_rendered_page()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx, pageSize: 2)));

        Assert.Equal(2, ctx.PageCount);
        cut.InvokeAsync(() => ctx.NextPageAsync()).GetAwaiter().GetResult();
        Assert.Equal(1, ctx.Page);

        // Clamped, not thrown or wrapped.
        cut.InvokeAsync(() => ctx.GoToPageAsync(99)).GetAwaiter().GetResult();
        Assert.Equal(1, ctx.Page);

        cut.InvokeAsync(() => ctx.SetPageSizeAsync(4)).GetAwaiter().GetResult();
        Assert.Equal(4, ctx.PageSize);
        Assert.Equal(0, ctx.Page);
    }

    [Fact]
    public void Changed_reports_what_changed_and_nothing_else()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx)));

        var seen = DataGridChange.None;
        ctx.Changed += c => seen |= c;

        cut.InvokeAsync(() => ctx.SortByAsync("Name")).GetAwaiter().GetResult();
        Assert.True(seen.HasFlag(DataGridChange.Sort));
        Assert.False(seen.HasFlag(DataGridChange.Selection));

        seen = DataGridChange.None;
        cut.InvokeAsync(() => ctx.FilterAsync("Name", "a")).GetAwaiter().GetResult();
        Assert.True(seen.HasFlag(DataGridChange.Filter));
        Assert.False(seen.HasFlag(DataGridChange.Sort));
    }

    [Fact]
    public void Selection_is_drivable_and_observable()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx)));
        var seen = DataGridChange.None;
        ctx.Changed += c => seen |= c;

        cut.InvokeAsync(() => ctx.SetSelectionAsync([_rows[0], _rows[1]])).GetAwaiter().GetResult();

        Assert.Equal(2, ctx.SelectedItems.Count);
        Assert.True(seen.HasFlag(DataGridChange.Selection));

        cut.InvokeAsync(() => ctx.ClearSelectionAsync()).GetAwaiter().GetResult();
        Assert.Empty(ctx.SelectedItems);
    }

    [Fact]
    public void Reset_clears_sorts_and_filters_together()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx)));

        cut.InvokeAsync(() => ctx.SortByAsync("Name")).GetAwaiter().GetResult();
        cut.InvokeAsync(() => ctx.SetQuickFilterAsync("a")).GetAwaiter().GetResult();
        cut.InvokeAsync(() => ctx.ResetAsync()).GetAwaiter().GetResult();

        Assert.Empty(ctx.Sorts);
        Assert.Null(ctx.QuickFilterText);
        Assert.Equal(4, cut.FindAll("tbody tr").Count);
    }

    [Fact]
    public void Detached_context_answers_instead_of_throwing()
    {
        var ctx = new DataGridContext<Row>();

        Assert.False(ctx.IsAttached);
        Assert.Empty(ctx.Columns);
        Assert.Empty(ctx.Sorts);
        Assert.Empty(ctx.SelectedItems);
        Assert.Equal(0, ctx.PageCount);
        Assert.Null(ctx.AdvancedFilter);

        // Commands are inert rather than fatal: a control can render before its grid exists.
        ctx.SortByAsync("Name").GetAwaiter().GetResult();
        ctx.GoToPageAsync(3).GetAwaiter().GetResult();
        ctx.ClearFiltersAsync().GetAwaiter().GetResult();
    }

    [Fact]
    public void Context_detaches_when_the_grid_goes_away()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx)));
        Assert.True(ctx.IsAttached);

        cut.Render(p => p.Add(x => x.Body, (RenderFragment)(b => { })));

        Assert.False(ctx.IsAttached);
    }

    [Fact]
    public void One_context_cannot_drive_two_grids()
    {
        var ctx = new DataGridContext<Row>();
        Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx)));

        var ex = Assert.Throws<InvalidOperationException>(
            () => Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx))));
        Assert.Contains("one grid", ex.Message);
    }

    [Fact]
    public void Snapshot_carries_the_state_a_query_editor_needs()
    {
        var ctx = new DataGridContext<Row>();
        var cut = Render<TestHost>(p => p.Add(x => x.Body, Grid(ctx, pageSize: 2)));

        cut.InvokeAsync(() => ctx.SortByAsync("Qty")).GetAwaiter().GetResult();
        cut.InvokeAsync(() => ctx.FilterAsync("Name", "a")).GetAwaiter().GetResult();

        var request = ctx.ToRequest();
        Assert.Equal("Qty", request.SortKey);
        Assert.Equal(2, request.PageSize);
        Assert.Contains(request.FilterModel, f => f.Key == "Name" && f.Value == "a");

        var snapshot = ctx.Snapshot();
        Assert.Equal(2, snapshot.Columns.Count);
        Assert.Equal("a", snapshot.Filters["Name"]);
    }

    /// <summary>Hosts a fragment so the grid can be added and removed without tearing down the test.</summary>
    private sealed class TestHost : ComponentBase
    {
        [Parameter] public RenderFragment? Body { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            if (Body is not null) builder.AddContent(0, Body);
        }
    }
}
