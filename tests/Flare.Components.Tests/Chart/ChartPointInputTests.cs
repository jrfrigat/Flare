using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class ChartPointInputTests : FlareTestContext
{
    private static readonly ChartData Points = new([
        new ChartSeries("First", []) { Points = [new(double.NaN, 1), new(0, 0, 0), new(3, 4, 9)] },
        new ChartSeries("Second", []) { Points = [new(1, 2, 4), new(2, double.PositiveInfinity)] }]);
    private static readonly ChartData Cells = new([new("First", [0, double.NaN, 3]), new("Second", [2])], ["A", "B", "C"]);

    [Theory]
    [InlineData(ChartType.Scatter)]
    [InlineData(ChartType.Bubble)]
    public void PointHitsMatchVisibleMarksAndRetainZero(ChartType type)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Points));
        var hits = cut.FindAll("circle[data-flare-chart-hit]");
        Assert.Equal(3, hits.Count);
        var marks = cut.FindAll("svg circle:not([fill='transparent'])");
        for (int i = 0; i < hits.Count; i++)
            foreach (var attribute in new[] { "cx", "cy", "r" })
                Assert.Equal(marks[i].GetAttribute(attribute), hits[i].GetAttribute(attribute));
        Assert.Contains("X: 0", hits[0].GetAttribute("aria-label"));
        Assert.Single(hits, h => h.GetAttribute("tabindex") == "0");
    }

    [Theory]
    [InlineData(ChartType.Scatter)]
    [InlineData(ChartType.Bubble)]
    public void PointActivationPreservesOriginalIndexAndDescribesCoordinates(ChartType type)
    {
        int? selected = null;
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Points)
            .Add(x => x.OnPointClick, (int i) => selected = i));
        cut.Find("[data-flare-chart-hit='point:0:2']").Click(new MouseEventArgs { ClientX = 200, ClientY = 100 });
        Assert.Equal(2, selected);
        var text = cut.Find("[role=tooltip]").TextContent;
        Assert.Contains("First", text); Assert.Contains("X: 3", text); Assert.Contains("Y: 4", text);
        if (type == ChartType.Bubble) Assert.Contains("9", text);
    }

    [Theory]
    [InlineData(ChartType.Scatter)]
    [InlineData(ChartType.Bubble)]
    public void HiddenPointSeriesLeaveNoHitsAndFitAllKeepsGeometry(ChartType type)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Points).Add(x => x.ScaleMode, ChartScaleMode.FitAll));
        var before = new[] { "cx", "cy", "r" }.Select(a => cut.Find("[data-flare-chart-hit='point:0:2']").GetAttribute(a)).ToArray();
        cut.FindAll($".{Css.Classes.Chart.LegendItem}")[1].Click();
        Assert.Equal(2, cut.FindAll("[data-flare-chart-hit]").Count);
        Assert.Equal(before, new[] { "cx", "cy", "r" }.Select(a => cut.Find("[data-flare-chart-hit='point:0:2']").GetAttribute(a)).ToArray());
    }

    [Theory]
    [InlineData(ChartType.Scatter)]
    [InlineData(ChartType.Bubble)]
    public void ZoomExcludesOffscreenPointControlsButPreservesTable(ChartType type)
    {
        var data = new ChartData([new ChartSeries("Points", [0, 0, 0]) { Points = [new(0, 0), new(10, 1), new(20, 2)] }]);
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, data).Add(x => x.Zoomable, true)
            .Add(x => x.Zoom, new ChartZoom(5, 15)).Add(x => x.DataTable, true));
        var hit = Assert.Single(cut.FindAll("[data-flare-chart-hit]"));
        Assert.Equal("point:0:1", hit.GetAttribute("data-flare-chart-hit"));
        Assert.Equal(3, cut.FindAll("tbody tr").Count);
    }

    [Fact]
    public void CellHitsMatchPresentCellsAndReportOriginalColumn()
    {
        int? selected = null;
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.HeatMap).Add(x => x.Data, Cells)
            .Add(x => x.OnPointClick, (int i) => selected = i));
        var hits = cut.FindAll("rect[data-flare-chart-hit]"); Assert.Equal(3, hits.Count);
        var marks = cut.FindAll($".{Css.Classes.Chart.Cell}");
        for (int i = 0; i < hits.Count; i++)
            foreach (var attribute in new[] { "x", "y", "width", "height" })
                Assert.Equal(marks[i].GetAttribute(attribute), hits[i].GetAttribute(attribute));
        Assert.Contains("0", hits[0].GetAttribute("aria-label"));
        cut.Find("[data-flare-chart-hit='cell:0:2']").Click(new MouseEventArgs());
        Assert.Equal(2, selected);Assert.Contains("C", cut.Find("[role=tooltip]").TextContent);Assert.Contains("First: 3", cut.Find("[role=tooltip]").TextContent);
    }

    [Fact]
    public void HiddenCellRowsPackLikeTheRendererAndKeepRawTable()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Cells).Add(x => x.DataTable, true));
        cut.FindAll($".{Css.Classes.Chart.LegendItem}")[0].Click();
        cut.Render(p => p.Add(x => x.Type, ChartType.HeatMap));
        var hit = Assert.Single(cut.FindAll("[data-flare-chart-hit]"));
        Assert.Equal("cell:1:0", hit.GetAttribute("data-flare-chart-hit"));
        Assert.Equal(cut.Find($".{Css.Classes.Chart.Cell}").GetAttribute("y"), hit.GetAttribute("y"));
        Assert.Contains("First", cut.Find("table").TextContent);
    }
}
