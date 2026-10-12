using System.Globalization;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class ChartZoomInputTests : FlareTestContext
{
    private static readonly ChartData Data = new([new("Values", Enumerable.Range(0, 10).Select(i => (double)i + 1).ToArray())]);
    private static double Number(AngleSharp.Dom.IElement element, string name) =>
        double.Parse(element.GetAttribute(name)!, CultureInfo.InvariantCulture);

    [Theory]
    [InlineData(ChartType.Line)]
    [InlineData(ChartType.Area)]
    [InlineData(ChartType.Bar)]
    [InlineData(ChartType.StackedBar)]
    [InlineData(ChartType.Combo)]
    public void ZoomHitsStayInsidePlotAndKeepOriginalIndices(ChartType type)
    {
        int? selected = null;
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Data)
            .Add(x => x.Zoomable, true).Add(x => x.Zoom, new ChartZoom(3, 6))
            .Add(x => x.OnPointClick, (int i) => selected = i));
        var hits = cut.FindAll("[data-flare-chart-hit]");
        Assert.Equal(new[] { "category:3", "category:4", "category:5", "category:6" },
            hits.Select(h => h.GetAttribute("data-flare-chart-hit")));
        var clip = cut.Find("clipPath rect");
        Assert.All(hits, h => {
            Assert.True(Number(h, "x") >= Number(clip, "x"));
            Assert.True(Number(h, "x") + Number(h, "width") <= Number(clip, "x") + Number(clip, "width") + 0.01);
        });
        Assert.Single(hits, h => h.GetAttribute("tabindex") == "0");
        hits[0].Click(new MouseEventArgs()); Assert.Equal(3, selected);
    }

    [Fact]
    public void PartialCategorySlotsAreTrimmedRatherThanRemoved()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data).Add(x => x.Zoomable, true)
            .Add(x => x.Zoom, new ChartZoom(3.25, 5.75)));
        var hits = cut.FindAll("[data-flare-chart-hit]");
        Assert.Equal(4, hits.Count);
        Assert.Equal(Number(cut.Find("clipPath rect"), "x"), Number(hits[0], "x"));
        Assert.True(Number(hits[0], "width") < Number(hits[1], "width"));
    }

    [Fact]
    public void HorizontalBarKeepsEveryRenderedRowWhenZoomed()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Bar).Add(x => x.Horizontal, true)
            .Add(x => x.Data, Data).Add(x => x.Zoomable, true).Add(x => x.Zoom, new ChartZoom(3, 6)));
        Assert.Equal(10, cut.FindAll("[data-flare-chart-hit]").Count);
    }

    [Theory]
    [InlineData(ChartType.Scatter)]
    [InlineData(ChartType.Bubble)]
    public void ZoomedPointHitsShareVisualClip(ChartType type)
    {
        var data = new ChartData([new ChartSeries("Points", [1, 2, 3]) { Points = [new(0, 1), new(10, 2, 9), new(20, 3)] }]);
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, data)
            .Add(x => x.Zoomable, true).Add(x => x.Zoom, new ChartZoom(10, 15)));
        var hit = Assert.Single(cut.FindAll("[data-flare-chart-hit]"));
        Assert.Equal("point:0:1", hit.GetAttribute("data-flare-chart-hit"));
        Assert.Equal($"url(#{cut.Find("clipPath").Id})", hit.ParentElement!.GetAttribute("clip-path"));
    }

    [Fact]
    public async Task ZoomingAwayFromFocusedCategoryDismissesItsDetails()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data).Add(x => x.Zoomable, true));
        await cut.Instance.ChartHitFocused("category:0", 100, 100);
        cut.Render(p => p.Add(x => x.Zoom, new ChartZoom(3, 6)));
        Assert.Empty(cut.FindAll("[role=tooltip]"));
    }

    [Theory]
    [InlineData(ChartType.Scatter)]
    [InlineData(ChartType.Bubble)]
    public void PointsOnlyDataEnablesZoom(ChartType type)
    {
        var data = new ChartData([new ChartSeries("Points", []) { Points = [new(0, 1), new(10, 2)] }]);
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, data).Add(x => x.Zoomable, true));
        Assert.Single(cut.FindAll($".{Css.Classes.Chart.Toolbar}"));
    }

    [Fact]
    public void HorizontalBarsDoNotAdvertiseUnsupportedZoom()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Bar).Add(x => x.Horizontal, true)
            .Add(x => x.Data, Data).Add(x => x.Zoomable, true));
        Assert.Empty(cut.FindAll($".{Css.Classes.Chart.Toolbar}"));
    }

    [Fact]
    public async Task GestureUsesViewBoxCoordinatesAndCancelRestoresWindow()
    {
        ChartZoom? window = null;
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data).Add(x => x.Zoomable, true)
            .Add(x => x.ZoomChanged, (ChartZoom? value) => window = value));
        var plot = cut.Find($".{Css.Classes.Chart.Plot}");
        double left = Number(plot, "data-plot-left"), width = Number(plot, "data-plot-width");
        await cut.Instance.ChartZoomGesture("start", left + width / 3);
        await cut.Instance.ChartZoomGesture("end", left + width * 2 / 3);
        Assert.NotNull(window); Assert.Equal(3, window.Value.From, 6); Assert.Equal(6, window.Value.To, 6);
        cut.Render(p => p.Add(x => x.Zoom, window));
        await cut.Instance.ChartZoomGesture("start", left + width / 2);
        await cut.Instance.ChartZoomGesture("move", left + width / 3);
        Assert.Equal(3.5, window!.Value.From, 6);
        await cut.Instance.ChartZoomGesture("cancel", left);
        Assert.Equal(3, window!.Value.From, 6); Assert.Equal(6, window.Value.To, 6);
    }

    [Fact]
    public async Task WheelKeepsItsSvgAnchorAndRejectsNonfiniteInput()
    {
        ChartZoom? window = null;
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data).Add(x => x.Zoomable, true)
            .Add(x => x.ZoomChanged, (ChartZoom? value) => window = value));
        var plot = cut.Find($".{Css.Classes.Chart.Plot}");
        double left = Number(plot, "data-plot-left"), width = Number(plot, "data-plot-width");
        await cut.Instance.ChartZoomWheel(double.NaN, left); Assert.Null(window);
        await cut.Instance.ChartZoomWheel(-100, left + width / 4);
        Assert.NotNull(window); Assert.Equal(0.45, window.Value.From, 6); Assert.Equal(7.65, window.Value.To, 6);
        var before = window;
        await cut.Instance.ChartZoomGesture("start", double.PositiveInfinity); Assert.Equal(before, window);
    }
}
