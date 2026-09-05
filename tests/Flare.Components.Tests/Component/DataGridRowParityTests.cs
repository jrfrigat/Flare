using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests.Component;

/// <summary>
/// A row is the same row whichever branch decided to render it. The grid used to carry two row
/// renderers - one for the plain path, a poorer one for the virtual path - and the second silently
/// lacked the tree indent and toggle, the detail row, row reorder and every aria attribute. Turning on
/// a parameter that only decides how many rows live in the DOM changed what a row WAS.
///
/// These tests pin the parity down from both sides: what the virtual path was missing, and what the
/// plain path was missing - the tree toggle lived only in the virtual renderer, so the Gallery's own
/// tree demo (paged, not virtual) rendered a flat list with no expanders at all.
/// </summary>
public sealed class DataGridRowParityTests : FlareTestContext
{
    private sealed record Row(string Name);

    private sealed class Node
    {
        public required string Name { get; init; }
        public List<Node> Children { get; init; } = [];
    }

    private static IEnumerable<Row> Rows(int count) =>
        Enumerable.Range(1, count).Select(i => new Row("row " + i));

    private static RenderFragment NameColumn() => b =>
    {
        b.OpenComponent<FlareColumn<Row>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Row, object?>)(r => r.Name));
        b.AddAttribute(3, "Sortable", true);
        b.CloseComponent();
    };

    // Two groups over twenty rows: "row 1".."row 9" and "row 10".."row 20".
    private static RenderFragment GroupByFirstWord() => b =>
    {
        b.OpenComponent<DataGridGroup<Row>>(0);
        b.AddAttribute(1, "Key", "Length");
        b.AddAttribute(2, "Selector", (Func<Row, object?>)(r => r.Name.Length));
        b.CloseComponent();
    };

    private static RenderFragment NodeColumn() => b =>
    {
        b.OpenComponent<FlareColumn<Node>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Field", (Func<Node, object?>)(n => n.Name));
        b.CloseComponent();
    };

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EveryRow_CarriesItsAriaAttributes(bool virtualized)
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(20))
            .Add(x => x.Virtual, virtualized)
            .Add(x => x.SelectionMode, SelectionMode.Multiple)
            .Add(x => x.Columns, NameColumn()));

        var row = cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}")[0];
        Assert.Equal("row", row.GetAttribute("role"));
        Assert.Equal("2", row.GetAttribute("aria-rowindex"));
        Assert.Equal("false", row.GetAttribute("aria-selected"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void EveryRow_IsDraggableWhenReorderable(bool virtualized)
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(20))
            .Add(x => x.Virtual, virtualized)
            .Add(x => x.RowReorderable, true)
            .Add(x => x.Columns, NameColumn()));

        var row = cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}")[0];
        Assert.NotNull(row.GetAttribute("data-flare-drag"));
        Assert.Equal("flare-datagrid-rows", row.GetAttribute("data-flare-drag-group"));
        Assert.Contains(Css.Classes.Drag.Item, row.ClassName, StringComparison.Ordinal);
    }

    // The tree toggle used to exist only in the virtual renderer, so a paged tree grid - which is what
    // the Gallery demo and any ordinary page builds - rendered a flattened list with no way to collapse
    // anything and no indentation.
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void TreeRows_HaveTheirToggleInEveryPath(bool virtualized)
    {
        var root = new Node
        {
            Name = "root",
            Children = [new Node { Name = "child a" }, new Node { Name = "child b" }],
        };

        var cut = Render<FlareDataGrid<Node>>(p => p
            .Add(x => x.Items, new[] { root }.AsEnumerable())
            .Add(x => x.Virtual, virtualized)
            .Add(x => x.Tree, new DataGridTreeConfig<Node>
            {
                ChildrenSelector = n => n.Children,
                InitiallyExpanded = _ => true,
            })
            .Add(x => x.Columns, NodeColumn()));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.DataGrid.TreeToggle}"));
        Assert.Contains("child a", cut.Markup, StringComparison.Ordinal);
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void DetailRows_ExpandInEveryPath(bool virtualized)
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(20))
            .Add(x => x.Virtual, virtualized)
            .Add(x => x.RowDetailTemplate, (RenderFragment<Row>)(r => b => b.AddContent(0, "detail of " + r.Name)))
            .Add(x => x.Columns, NameColumn()));

        Assert.Empty(cut.FindAll($"tr.{Css.Classes.DataGrid.DetailRow}"));

        cut.FindAll($"button.{Css.Classes.DataGrid.DetailBtn}")[0].Click();

        Assert.Single(cut.FindAll($"tr.{Css.Classes.DataGrid.DetailRow}"));
        Assert.Contains("detail of row 1", cut.Markup, StringComparison.Ordinal);
    }

    // Grouping decides which LINES there are; virtualization decides how many of them are in the DOM.
    // The virtual branch used to sit above the grouped one, so switching virtualization on threw the
    // grouping away without a word and rendered a flat list.
    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void Grouping_SurvivesVirtualization(bool virtualized)
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(20))
            .Add(x => x.Virtual, virtualized)
            .Add(x => x.Height, "300px")
            .Add(x => x.Grouping, GroupByFirstWord())
            .Add(x => x.Columns, NameColumn()));

        Assert.NotEmpty(cut.FindAll($"tr.{Css.Classes.DataGrid.GroupHeader}"));
    }

    // The grouped branch carried a THIRD copy of the row markup, and the poorest of the three: no aria,
    // no reorder, no detail row, no cell selection, and RenderCell instead of RenderCellOrInput - so a
    // grouped grid could not be edited in place either.
    [Fact]
    public void GroupedRows_AreOrdinaryRows()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(6))
            .Add(x => x.RowReorderable, true)
            .Add(x => x.SelectionMode, SelectionMode.Multiple)
            .Add(x => x.Grouping, GroupByFirstWord())
            .Add(x => x.Columns, NameColumn()));

        var row = cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row}")[0];
        Assert.Equal("row", row.GetAttribute("role"));
        Assert.NotNull(row.GetAttribute("aria-rowindex"));
        Assert.Equal("false", row.GetAttribute("aria-selected"));
        Assert.NotNull(row.GetAttribute("data-flare-drag"));
    }

    // Expansion used to be stored by the row's POSITION on the page, so sorting kept the same slot open
    // and the wrong row's detail appeared under a different row's data.
    [Fact]
    public void DetailRow_StaysWithItsRowAcrossSorting()
    {
        var cut = Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, Rows(3))
            .Add(x => x.RowKey, (Func<Row, object>)(r => r.Name))
            .Add(x => x.RowDetailTemplate, (RenderFragment<Row>)(r => b => b.AddContent(0, "detail of " + r.Name)))
            .Add(x => x.Columns, NameColumn()));

        cut.FindAll($"button.{Css.Classes.DataGrid.DetailBtn}")[0].Click();
        Assert.Contains("detail of row 1", cut.Markup, StringComparison.Ordinal);

        // Sort descending: row 1 moves to the bottom, and its detail must move with it.
        cut.Find($"th.{Css.Classes.DataGrid.ThSortable}").Click();
        cut.Find($"th.{Css.Classes.DataGrid.ThSortable}").Click();

        Assert.Contains("detail of row 1", cut.Markup, StringComparison.Ordinal);
        Assert.DoesNotContain("detail of row 3", cut.Markup, StringComparison.Ordinal);
        Assert.Equal("true", cut.FindAll($"button.{Css.Classes.DataGrid.DetailBtn}")[^1].GetAttribute("aria-expanded"));
    }
}
