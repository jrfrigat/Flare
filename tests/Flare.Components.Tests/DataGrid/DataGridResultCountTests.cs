using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// The grid's status region states how many rows pass the filters. A page that filters outside the
/// grid hands it a new Items list, and the status must follow that list as it follows a column filter.
/// </summary>
public class DataGridResultCountTests : FlareTestContext
{
    private record Row(string Name);

    private static Row[] Rows(int count) => Enumerable.Range(1, count).Select(i => new Row($"Row {i}")).ToArray();

    private static RenderFragment NameColumn => columns =>
    {
        columns.OpenComponent<FlareColumn<Row>>(0);
        columns.AddAttribute(1, "Title", "Name");
        columns.AddAttribute(2, "Field", (Func<Row, object?>)(row => row.Name));
        columns.CloseComponent();
    };

    private static string Status(IRenderedComponent<FlareDataGrid<Row>> cut)
        => cut.Find("[role=status]").TextContent.Trim();

    private IRenderedComponent<FlareDataGrid<Row>> RenderGrid(IEnumerable<Row> items)
        => Render<FlareDataGrid<Row>>(p => p
            .Add(x => x.Items, items)
            .Add(x => x.Columns, NameColumn));

    [Fact]
    public void FirstRender_AnnouncesNothing()
    {
        var cut = RenderGrid(Rows(3));

        Assert.Equal(string.Empty, Status(cut));
    }

    [Fact]
    public void NewItems_WithAnotherCount_AnnounceTheNewCount()
    {
        var cut = RenderGrid(Rows(3));

        cut.Render(p => p.Add(x => x.Items, Rows(0)));
        Assert.Equal(string.Format(Resources.FlareStrings.DataGrid_AnnounceFiltered, 0), Status(cut));

        cut.Render(p => p.Add(x => x.Items, Rows(3)));
        Assert.Equal(string.Format(Resources.FlareStrings.DataGrid_AnnounceFiltered, 3), Status(cut));
    }

    [Fact]
    public void NewItems_WithTheSameCount_LeaveTheStatusAlone()
    {
        var cut = RenderGrid(Rows(3));

        cut.Render(p => p.Add(x => x.Items, Rows(3)));

        Assert.Equal(string.Empty, Status(cut));
    }
}
