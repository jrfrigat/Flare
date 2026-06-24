namespace Flare.Components.Tests;

// FlareChart - SVG shapes must carry colors via the style attribute (var() is not
// resolved in SVG presentation attributes like fill=/stroke=).
public class FlareChartTests : FlareTestContext
{
    private static readonly ChartData _data = new(
        [new ChartSeries("A", [1, 5, 3, 8])],
        ["q1", "q2", "q3", "q4"]);

    [Fact]
    public void Line_RendersPolyline_WithStrokeStyle()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data));

        var poly = cut.Find("polyline");
        Assert.Contains("stroke:", poly.GetAttribute("style") ?? "");
    }

    [Fact]
    public void Bar_RendersRects_WithFillStyle()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Bar)
            .Add(x => x.Data, _data));

        // visual bars carry the color via style:fill; transparent hit-zone rects do not
        var bars = cut.FindAll("rect").Where(r => (r.GetAttribute("style") ?? "").Contains("fill:")).ToList();
        Assert.NotEmpty(bars);
    }

    [Fact]
    public void Pie_RendersPaths_WithFillStyle()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Pie)
            .Add(x => x.Data, new ChartData([
                new ChartSeries("A", [3]),
                new ChartSeries("B", [7]),
            ])));

        // two visual slices carry style:fill (plus two transparent hit-zone paths)
        var fills = cut.FindAll("path").Where(p => (p.GetAttribute("style") ?? "").Contains("fill:")).ToList();
        Assert.Equal(2, fills.Count);
    }

    [Fact]
    public void LineAndBar_RenderCategoryHitZones()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Bar)
            .Add(x => x.Data, _data));

        // one transparent hit zone per category (4 in _data)
        var zones = cut.FindAll("rect").Where(r => (r.GetAttribute("fill") ?? "") == "transparent").ToList();
        Assert.Equal(4, zones.Count);
    }

    [Fact]
    public void NoColorAsPresentationAttribute_OnlyViaStyle()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data));

        // var() in a fill=/stroke= attribute would never resolve in a browser
        Assert.DoesNotContain("stroke=\"var(", cut.Markup);
        Assert.DoesNotContain("fill=\"var(", cut.Markup);
    }

    [Fact]
    public void Coordinates_UseInvariantDecimalSeparator_UnderCommaCulture()
    {
        var prev = System.Globalization.CultureInfo.CurrentCulture;
        try
        {
            // ru-RU formats 192.0 as "192,0" — invalid in SVG. The component must stay invariant.
            System.Globalization.CultureInfo.CurrentCulture = new System.Globalization.CultureInfo("ru-RU");
            var cut = Render<FlareChart>(p => p
                .Add(x => x.Type, ChartType.Line)
                .Add(x => x.Data, _data));

            var points = cut.Find("polyline").GetAttribute("points") ?? "";
            // x,y pairs are comma-joined; decimals must be dots, so no ",," and a "." present
            Assert.Contains(".", points);
            Assert.DoesNotContain(",,", points);
            Assert.DoesNotContain(", ", points);
        }
        finally
        {
            System.Globalization.CultureInfo.CurrentCulture = prev;
        }
    }

    [Fact]
    public void RendersLegendItems_PerSeries()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, new ChartData([
                new ChartSeries("One", [1, 2]),
                new ChartSeries("Two", [3, 4]),
            ])));

        Assert.Equal(2, cut.FindAll(".flare-chart__legend-item").Count);
    }
}
