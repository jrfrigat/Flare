namespace Flare.Components.Tests;

// FlareChart - SVG shapes must carry colors via the style attribute (var() is not
// resolved in SVG presentation attributes like fill=/stroke=).
public class FlareChartTests : FlareTestContext
{
    private static readonly ChartData _data = new(
        [new ChartSeries("A", [1, 5, 3, 8])],
        ["q1", "q2", "q3", "q4"]);

    [Fact]
    public void Line_RendersPath_WithStrokeStyle()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data));

        // The line is a stroked <path> (unified for straight/smooth/area); color via style.
        var path = cut.Find("path");
        Assert.Contains("stroke:", path.GetAttribute("style") ?? "");
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

            var d = cut.Find("path").GetAttribute("d") ?? "";
            // The path uses space-separated "M x y L x y" with dot decimals; a comma culture
            // would emit "192,0", so any comma means the invariant formatting leaked.
            Assert.Contains(".", d);
            Assert.DoesNotContain(",", d);
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

    // --- Phase 1 -------------------------------------------------------------------------------

    [Fact]
    public void Sparkline_IsChromeless_AndStretches()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Title, "Throughput")
            .Add(x => x.Data, _data)
            .Add(x => x.Sparkline, true));

        Assert.NotEmpty(cut.FindAll(".flare-chart--sparkline"));
        Assert.Empty(cut.FindAll(".flare-chart__legend"));       // no legend
        Assert.Empty(cut.FindAll(".flare-chart__title"));        // no title
        Assert.Empty(cut.FindAll("line"));                       // no grid lines
        Assert.Empty(cut.FindAll("text"));                       // no axis labels
        Assert.Equal("none", cut.Find("svg").GetAttribute("preserveAspectRatio"));
        Assert.Contains("non-scaling-stroke", cut.Find("path").GetAttribute("vector-effect") ?? "");
    }

    [Fact]
    public void Area_RendersGradientFilledPath()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.Area, true));

        Assert.NotEmpty(cut.FindAll("lineargradient"));           // a fade gradient is defined
        Assert.Contains(cut.FindAll("path"), p => (p.GetAttribute("fill") ?? "").StartsWith("url(#"));
    }

    [Fact]
    public void ShowLegend_False_HidesLegend()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.ShowLegend, false));

        Assert.Empty(cut.FindAll(".flare-chart__legend"));
    }

    [Fact]
    public void ShowGrid_False_HidesGridLines()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.ShowGrid, false));

        Assert.Empty(cut.FindAll("line"));
    }

    [Fact]
    public void ShowMarkers_RendersPointCircles()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.ShowMarkers, true));

        Assert.Equal(4, cut.FindAll("circle").Count); // one per data point
    }

    [Fact]
    public void LegendPosition_Top_RendersLegendBeforePlot()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.LegendPosition, ChartLegendPosition.Top));

        var root = cut.Find(".flare-chart");
        var children = root.Children.ToList();
        int legendIdx = children.FindIndex(c => c.ClassList.Contains("flare-chart__legend"));
        int plotIdx = children.FindIndex(c => c.ClassList.Contains("flare-chart__plot"));
        Assert.True(legendIdx >= 0 && plotIdx >= 0 && legendIdx < plotIdx);
    }

    // --- Phase 2: breadth types --------------------------------------------------------------

    [Fact]
    public void Area_Type_RendersGradientFill()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Area)
            .Add(x => x.Data, _data));

        Assert.NotEmpty(cut.FindAll("lineargradient"));
        Assert.Contains(cut.FindAll("path"), p => (p.GetAttribute("fill") ?? "").StartsWith("url(#"));
    }

    [Fact]
    public void StackedBar_RendersOneFillRect_PerSeriesPerCategory()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.StackedBar)
            .Add(x => x.Data, new ChartData([
                new ChartSeries("A", [1, 2, 3, 4]),
                new ChartSeries("B", [4, 3, 2, 1]),
            ], ["q1", "q2", "q3", "q4"])));

        var fills = cut.FindAll("rect").Where(r => (r.GetAttribute("style") ?? "").Contains("fill:")).ToList();
        Assert.Equal(8, fills.Count); // 2 series x 4 categories
    }

    [Fact]
    public void Scatter_RendersCircle_PerPoint()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Scatter)
            .Add(x => x.Data, new ChartData([
                new ChartSeries("S", System.Array.Empty<double>(), Points:
                [
                    new ChartPoint(1, 2), new ChartPoint(3, 5),
                    new ChartPoint(4, 1), new ChartPoint(6, 7),
                ]),
            ])));

        Assert.Equal(4, cut.FindAll("circle").Count);
    }

    [Fact]
    public void Radar_RendersFilledPolygon_PerSeries()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Radar)
            .Add(x => x.Data, new ChartData([
                new ChartSeries("A", [4, 6, 3, 7, 5]),
                new ChartSeries("B", [6, 3, 7, 4, 6]),
            ], ["Speed", "Power", "Range", "Cost", "Eco"])));

        var seriesPolys = cut.FindAll("polygon").Where(p => (p.GetAttribute("style") ?? "").Contains("fill-opacity")).ToList();
        Assert.Equal(2, seriesPolys.Count);
    }
}
