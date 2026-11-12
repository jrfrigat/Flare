using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// Rows are not DOM-keyed unless RowKey is set, so a sort patches them in place instead of moving
/// keyed nodes. The cells must still show the rows in their new order in both modes.
/// </summary>
public sealed class DataGridRowKeyTests : FlareTestContext
{
    private sealed class Row(string name)
    {
        public string Name { get; } = name;
    }

    private static RenderFragment Columns() => b =>
    {
        b.OpenComponent<FlareColumn<Row>>(0);
        b.AddAttribute(1, "Title", "Name");
        b.AddAttribute(2, "Sortable", true);
        b.AddAttribute(3, "Template", (RenderFragment<Row>)(r => builder => builder.AddContent(0, r.Name)));
        b.CloseComponent();
    };

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid(Func<Row, object>? rowKey) =>
        Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, new List<Row> { new("b"), new("a") })
            .Add(x => x.RowKey, rowKey)
            .Add(x => x.Columns, Columns()));

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void SortingUpdatesTheCellsInBothKeyingModes(bool withRowKey)
    {
        var cut = RenderGrid(withRowKey ? row => row.Name : null);

        cut.Find($"th.{Css.Classes.DataGrid.ThSortable}").Click();

        var cells = cut.FindAll($"tbody tr.{Css.Classes.DataGrid.Row} td");
        Assert.Equal("a", cells[0].TextContent.Trim());
        Assert.Equal("b", cells[1].TextContent.Trim());
    }
}
