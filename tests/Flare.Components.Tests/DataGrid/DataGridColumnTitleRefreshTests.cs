using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;

namespace Flare.Components.Tests;

public class DataGridColumnTitleRefreshTests : FlareTestContext
{
    private sealed record Row(string Name);

    private static readonly Row[] _rows = [new("Alpha")];

    private sealed class Host : ComponentBase
    {
        [Parameter] public string Title { get; set; } = "";

        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            b.OpenComponent<FlareDataGrid<Row>>(0);
            b.AddComponentParameter(1, nameof(FlareDataGrid<Row>.Items), _rows);
            b.AddComponentParameter(2, nameof(FlareDataGrid<Row>.Columns), (RenderFragment)(cols =>
            {
                cols.OpenComponent<FlareColumn<Row>>(0);
                cols.AddComponentParameter(1, nameof(FlareColumnBase.Title), Title);
                cols.AddComponentParameter(2, nameof(FlareColumn<Row>.Field), (Func<Row, object?>)(r => r.Name));
                cols.CloseComponent();
            }));
            b.CloseComponent();
        }
    }

    [Fact]
    public void AChangedTitle_UpdatesTheHeaderAndTheColumnList()
    {
        var cut = Render<Host>(p => p.Add(x => x.Title, "Статус"));
        Assert.Equal("Статус", cut.Find("thead th").TextContent.Trim());

        cut.Render(p => p.Add(x => x.Title, "Status"));

        Assert.Equal("Status", cut.Find("thead th").TextContent.Trim());
        var grid = cut.FindComponent<FlareDataGrid<Row>>().Instance;
        Assert.Equal(["Status"], grid.PickableColumns.Select(c => c.Title));
    }

    [Fact]
    public void AnUnchangedTitle_KeepsTheSameProjection()
    {
        var cut = Render<Host>(p => p.Add(x => x.Title, "Name"));

        cut.Render(p => p.Add(x => x.Title, "Name"));

        Assert.Equal("Name", cut.Find("thead th").TextContent.Trim());
    }
}
