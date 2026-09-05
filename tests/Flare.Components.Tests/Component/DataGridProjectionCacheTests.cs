using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// The grid's sorted/filtered projection is cached until the data changes, not until the render ends.
/// It used to be thrown away at both render-cycle boundaries, so every render the grid caused for itself
/// - a row selected, a detail row opened, a cell focused - re-ran the whole filter and sort over the
/// entire set. On a hundred rows that is invisible; on the sets this grid is now expected to hold it IS
/// the render.
///
/// Counted through the quick-filter predicate rather than through the data: the pipeline is the only
/// thing that calls it, where rendering reads the item itself on every pass regardless.
/// </summary>
public sealed class DataGridProjectionCacheTests : FlareTestContext
{
    private sealed record Row(string Name);

    private static IEnumerable<Row> Rows(int count) =>
        Enumerable.Range(1, count).Select(i => new Row($"row {i:000}")).ToList();

    private static RenderFragment NameColumn() => b =>
    {
        b.OpenComponent<FlareColumn<Row>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Row, object?>)(r => r.Name));
        b.AddAttribute(3, "Sortable", true);
        b.CloseComponent();
    };

    // A render the grid causes for ITSELF must not re-run the pipeline. A parameter set still does -
    // that is the net that catches an application mutating its own list - so this has to be a
    // grid-internal render, and selecting a row is the simplest one.
    [Fact]
    public void AGridInternalRenderDoesNotReRunThePipeline()
    {
        var calls = 0;
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(50))
            .Add(x => x.SelectionMode, SelectionMode.Multiple)
            .Add(x => x.QuickFilter, (Func<Row, bool>)(_ => { calls++; return true; }))
            .Add(x => x.Columns, NameColumn()));

        var afterFirstRender = calls;
        Assert.True(afterFirstRender > 0, "The pipeline has to have run at least once.");

        cut.FindAll("tbody tr.flare-datagrid__row")[0].Click();
        cut.FindAll("tbody tr.flare-datagrid__row")[1].Click();

        Assert.Equal(afterFirstRender, calls);
    }

    [Fact]
    public void SortingReRunsIt()
    {
        var calls = 0;
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(20))
            .Add(x => x.QuickFilter, (Func<Row, bool>)(_ => { calls++; return true; }))
            .Add(x => x.Columns, NameColumn()));

        var before = calls;
        cut.Find("th.flare-datagrid__th--sortable").Click();

        Assert.True(calls > before, "A sort changes the projection, so the cache must be dropped.");
        Assert.Equal("row 001", cut.FindAll("tbody tr.flare-datagrid__row")[0].TextContent.Trim());

        cut.Find("th.flare-datagrid__th--sortable").Click();
        Assert.Equal("row 020", cut.FindAll("tbody tr.flare-datagrid__row")[0].TextContent.Trim());
    }

    // The case the cache could get wrong and no test would notice: the application mutates the list it
    // already handed over, so nothing about the grid's own state says the data moved. The parameter set
    // is what catches it, which is why every path clears through one method rather than by hand.
    [Fact]
    public void MutatingTheCallersOwnListIsPickedUp()
    {
        var rows = new List<Row>(Rows(3));
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, rows)
            .Add(x => x.Columns, NameColumn()));

        Assert.Equal(3, cut.FindAll("tbody tr.flare-datagrid__row").Count);

        rows.Add(new Row("row 004"));
        cut.Render();

        Assert.Equal(4, cut.FindAll("tbody tr.flare-datagrid__row").Count);
        Assert.Contains("row 004", cut.Markup, StringComparison.Ordinal);
    }

    [Fact]
    public void ReplacingTheItemsIsPickedUp()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(3))
            .Add(x => x.Columns, NameColumn()));

        cut.Render(p => p.Add(x => x.Items, Rows(7)));

        Assert.Equal(7, cut.FindAll("tbody tr.flare-datagrid__row").Count);
    }

    [Fact]
    public void FilteringIsPickedUp()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(20))
            .Add(x => x.FilterMode, DataGridFilterMode.Simple)
            .Add(x => x.Columns, FilterableNameColumn()));

        Assert.Equal(20, cut.FindAll("tbody tr.flare-datagrid__row").Count);

        cut.Find(".flare-datagrid__filter-row .flare-input__control").Input("row 007");

        Assert.Equal(1, cut.FindAll("tbody tr.flare-datagrid__row").Count);
    }

    private static RenderFragment FilterableNameColumn() => b =>
    {
        b.OpenComponent<FlareColumn<Row>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Row, object?>)(r => r.Name));
        b.AddAttribute(3, "Filterable", true);
        // The filter is debounced; zero applies it synchronously, which is what a test can observe.
        b.AddAttribute(4, "FilterDebounceMs", 0);
        b.CloseComponent();
    };
}
