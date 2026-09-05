using AngleSharp.Dom;

namespace Flare.Components.Tests;

// Sizing, per-series line options, FlareColor series colors, the directional annotations and the zoom
// window - the parts of FlareChart that are decided per SERIES or per WINDOW rather than per chart.
public class FlareChartSizingAndSeriesTests : FlareTestContext
{
    private static readonly ChartData _data = new(
        [new ChartSeries("A", [1, 5, 3, 8])],
        ["q1", "q2", "q3", "q4"]);

    private static string ViewBox(IRenderedComponent<FlareChart> cut) =>
        cut.Find("svg").GetAttribute("viewBox") ?? "";

    // ---- Sizing ------------------------------------------------------------------------------------

    [Fact]
    public void Width_SetsTheAuthoredAspectRatio()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Data, _data)
            .Add(x => x.Fluid, false)
            .Add(x => x.Width, 1200)
            .Add(x => x.Height, 200));

        Assert.Equal("0 0 1200 200", ViewBox(cut));
    }

    [Fact]
    public void WithoutAMeasurement_FluidFallsBackToTheAuthoredAspectRatio()
    {
        // bUnit has no layout, so the ResizeObserver never reports: this is the prerender/no-JS path,
        // which must still draw a correct chart rather than a letterboxed one.
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Data, _data)
            .Add(x => x.Width, 640)
            .Add(x => x.Height, 180));

        Assert.Equal("0 0 640 180", ViewBox(cut));
        Assert.DoesNotContain("height:180px", cut.Find("svg").GetAttribute("style") ?? "");
    }

    [Fact]
    public void NonFluidChart_DoesNotPinTheSvgHeight()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Data, _data)
            .Add(x => x.Fluid, false));

        Assert.True(string.IsNullOrEmpty(cut.Find("svg").GetAttribute("style")));
    }

    // ---- Per-series line options -------------------------------------------------------------------

    [Fact]
    public void SmoothIsPerSeries_SoOneChartMixesStraightAndCurvedLines()
    {
        var data = new ChartData(
        [
            new ChartSeries("straight", [1, 5, 3, 8], Smooth: false),
            new ChartSeries("curved", [2, 4, 6, 5], Smooth: true),
        ], ["q1", "q2", "q3", "q4"]);

        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Line).Add(x => x.Data, data));

        var lines = cut.FindAll($"path.{Css.Classes.Chart.Line}").Select(l => l.GetAttribute("d") ?? "").ToList();
        Assert.Equal(2, lines.Count);
        Assert.DoesNotContain(lines, d => d.StartsWith('M') && d.Contains(" C ") && d == lines[0]);
        Assert.Contains(lines, d => d.Contains(" C "));   // the smoothed one is a cubic path
        Assert.Contains(lines, d => !d.Contains(" C "));  // the straight one is not
    }

    [Fact]
    public void ChartLevelSmooth_StillAppliesToASeriesThatDoesNotOverrideIt()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.Smooth, true));

        Assert.Contains(" C ", cut.Find($"path.{Css.Classes.Chart.Line}").GetAttribute("d") ?? "");
    }

    [Fact]
    public void Smooth_KeepsEverySegmentInsideItsOwnEndpoints_SoSpikesDoNotOvershoot()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Smooth, true)
            .Add(x => x.Data, new ChartData(
                [new ChartSeries("spiky", [0, 0, 0, 0, 6500, 0, 0, 3900, 0, 0, 0])])));

        var n = System.Text.RegularExpressions.Regex
            .Matches(cut.Find($"path.{Css.Classes.Chart.Line}").GetAttribute("d") ?? "", @"-?\d+(\.\d+)?")
            .Select(x => double.Parse(x.Value, System.Globalization.CultureInfo.InvariantCulture))
            .ToList();

        // "M y0" then one "C c1x c1y c2x c2y x y" per segment; both control points must sit
        // between the segment's own endpoints, which bounds the whole cubic there.
        for (int i = 2; i + 5 < n.Count; i += 6)
        {
            double from = n[i - 1], to = n[i + 5];
            double lo = Math.Min(from, to) - 0.06, hi = Math.Max(from, to) + 0.06;
            Assert.InRange(n[i + 1], lo, hi);
            Assert.InRange(n[i + 3], lo, hi);
        }
    }

    [Fact]
    public void PatternedLine_DropsPathLength_SoTheDashArrayKeepsItsOwnUnits()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, new ChartData(
            [
                new ChartSeries("solid", [1, 2, 3]),
                new ChartSeries("dashed", [2, 3, 4], LineStyle: ChartLineStyle.Dashed),
            ])));

        var lines = cut.FindAll($"path.{Css.Classes.Chart.Line}");
        Assert.Equal("1", lines[0].GetAttribute("pathLength"));
        Assert.Null(lines[1].GetAttribute("pathLength"));
    }

    [Theory]
    [InlineData(ChartLineStyle.Dashed, Css.Tokens.Chart.LineDashDashed)]
    [InlineData(ChartLineStyle.Dotted, Css.Tokens.Chart.LineDashDotted)]
    [InlineData(ChartLineStyle.DashDot, Css.Tokens.Chart.LineDashDashDot)]
    public void LineStyle_ResolvesToTheThemesDashToken(ChartLineStyle style, string token)
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, new ChartData([new ChartSeries("A", [1, 2, 3], LineStyle: style)])));

        Assert.Contains(token, cut.Find($"path.{Css.Classes.Chart.Line}").GetAttribute("style") ?? "");
    }

    [Fact]
    public void SolidLine_WritesNoDashAtAll()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Line).Add(x => x.Data, _data));
        Assert.DoesNotContain("stroke-dasharray", cut.Find($"path.{Css.Classes.Chart.Line}").GetAttribute("style") ?? "");
    }

    [Fact]
    public void MarkersArePerSeries()
    {
        var data = new ChartData(
        [
            new ChartSeries("dots", [1, 5, 3, 8], ShowMarkers: true),
            new ChartSeries("bare", [2, 4, 6, 5]),
        ], ["q1", "q2", "q3", "q4"]);

        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Line).Add(x => x.Data, data));

        Assert.Equal(4, cut.FindAll("circle").Count);
    }

    // ---- Colors ------------------------------------------------------------------------------------

    [Fact]
    public void SeriesColor_AcceptsASemanticRole()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, new ChartData([new ChartSeries("A", [1, 2, 3], FlareColor.Error)])));

        Assert.Contains($"var({Css.Tokens.Color.Error})", cut.Find($"path.{Css.Classes.Chart.Line}").GetAttribute("style") ?? "");
    }

    [Fact]
    public void SeriesColor_StillAcceptsARawCssColor()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, new ChartData([new ChartSeries("A", [1, 2, 3], "#ff0000")])));

        Assert.Contains("#ff0000", cut.Find($"path.{Css.Classes.Chart.Line}").GetAttribute("style") ?? "");
    }

    [Fact]
    public void SeriesWithoutAColor_TakesThePaletteSlot()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data));

        Assert.Contains($"var({Css.Tokens.Chart.Series1})", cut.Find($"path.{Css.Classes.Chart.Line}").GetAttribute("style") ?? "");
    }

    // ---- Annotations -------------------------------------------------------------------------------

    [Fact]
    public void ArrowAnnotation_DrawsALineWithAnArrowheadMarker()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.Annotations, new[] { ChartAnnotation.Arrow(0, 1, 3, 8, "up") }));

        Assert.NotEmpty(cut.FindAll("marker"));
        Assert.Contains(cut.FindAll("line"), l => (l.GetAttribute("marker-end") ?? "").StartsWith("url(#"));
        Assert.Contains(cut.FindAll("text"), t => t.TextContent.Contains("up"));
    }

    [Fact]
    public void SegmentAnnotation_HasNoArrowhead()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.Annotations, new[] { ChartAnnotation.Segment(0, 1, 3, 8) }));

        Assert.Empty(cut.FindAll("marker"));
    }

    [Fact]
    public void PointAnnotation_DrawsAMarkedLabelledPoint()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.Annotations, new[] { ChartAnnotation.At(2, 3, "peak") }));

        Assert.NotEmpty(cut.FindAll("circle"));
        Assert.Contains(cut.FindAll("text"), t => t.TextContent.Contains("peak"));
    }

    [Fact]
    public void VerticalBandAnnotation_ShadesARangeOfCategories()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.Annotations, new[] { ChartAnnotation.VerticalBand(1, 2, "window") }));

        Assert.Contains(cut.FindAll("rect"),
            r => (r.GetAttribute("style") ?? "").Contains($"fill-opacity:var({Css.Tokens.Chart.AnnotationBandOpacity})"));
    }

    [Fact]
    public void AnnotationColor_AcceptsASemanticRole()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.Annotations, new[] { ChartAnnotation.Threshold(5, "t", FlareColor.Success) }));

        Assert.Contains(cut.FindAll("line"),
            l => (l.GetAttribute("style") ?? "").Contains($"stroke:var({Css.Tokens.Color.Success})"));
    }

    // ---- Zoom --------------------------------------------------------------------------------------

    [Fact]
    public void WithoutAZoom_TheFirstAndLastPointSitOnThePlotEdges()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.Fluid, false));

        var d = cut.Find($"path.{Css.Classes.Chart.Line}").GetAttribute("d") ?? "";
        Assert.StartsWith("M 36.0", d);       // left padding
        Assert.Contains("388.0", d);          // 400 - right padding
    }

    [Fact]
    public void AZoomWindow_SpreadsTheVisibleCategoriesAcrossThePlot()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.Fluid, false)
            .Add(x => x.Zoomable, true)
            .Add(x => x.Zoom, new ChartZoom(1, 2)));

        // Category 1 is now the left edge and category 2 the right edge, so the drawn path spans the
        // plot even though the data has four points.
        var d = cut.Find($"path.{Css.Classes.Chart.Line}").GetAttribute("d") ?? "";
        Assert.Contains("L 36.0", d);
        Assert.Contains("L 388.0", d);
    }

    [Fact]
    public void AZoomedChart_ClipsItsDataHorizontally()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.Fluid, false)
            .Add(x => x.Zoomable, true)
            .Add(x => x.Zoom, new ChartZoom(1, 2)));

        Assert.Contains(cut.FindAll("g"), g => (g.GetAttribute("clip-path") ?? "").StartsWith("url(#"));

        // Vertically the rect spans the whole drawing: zoom moves the X axis only, and a tight vertical
        // clip removes marks that overhang the plot on purpose - a bar's value label, half a marker.
        var rect = cut.Find("clipPath rect");
        Assert.Equal("0", rect.GetAttribute("y"));
        Assert.Equal("220", rect.GetAttribute("height"));
        Assert.Equal("36", rect.GetAttribute("x"));
    }

    [Fact]
    public void AnUnzoomedChart_IsNotClippedAtAll()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, _data)
            .Add(x => x.Zoomable, true));

        Assert.Empty(cut.FindAll("clipPath"));
        Assert.DoesNotContain(cut.FindAll("g"), g => (g.GetAttribute("clip-path") ?? "").Length > 0);
    }

    [Fact]
    public void BarValueLabelAtTheTopOfThePlot_IsNotClippedAway()
    {
        // The tallest bar reaches y = _padT and its value label is drawn 3 units ABOVE that, so a clip
        // rectangle starting at the plot top would swallow it.
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Bar)
            .Add(x => x.Data, new ChartData([new ChartSeries("A", [10, 20, 30])], ["a", "b", "c"]))
            .Add(x => x.Fluid, false)
            .Add(x => x.ShowValues, true));

        var label = cut.FindAll("text").First(t => t.TextContent.Trim() == "30" && t.GetAttribute("y") == "9.0");
        Assert.Null(label.Closest("g[clip-path]"));
    }

    [Fact]
    public void ZoomToolbar_AppearsOnlyForAZoomableChart()
    {
        var plain = Render<FlareChart>(p => p.Add(x => x.Data, _data));
        Assert.Empty(plain.FindAll($".{Css.Classes.Chart.Toolbar}"));

        var zoomable = Render<FlareChart>(p => p.Add(x => x.Data, _data).Add(x => x.Zoomable, true));
        Assert.Single(zoomable.FindAll($".{Css.Classes.Chart.Toolbar}"));
        Assert.Equal(3, zoomable.FindAll($".{Css.Classes.Chart.Toolbar} button").Count);
    }

    [Fact]
    public void ZoomingIn_NarrowsTheWindowAndReportsIt()
    {
        ChartZoom? reported = null;
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Data, _data)
            .Add(x => x.Zoomable, true)
            .Add(x => x.ZoomChanged, (ChartZoom? z) => reported = z));

        cut.FindAll($".{Css.Classes.Chart.Toolbar} button")[0].Click();

        Assert.NotNull(reported);
        Assert.True(reported!.Value.Span < 3);   // the full domain of four points is 3
    }

    [Fact]
    public void ResetButton_ClearsTheWindow()
    {
        ChartZoom? reported = new ChartZoom(1, 2);
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Data, _data)
            .Add(x => x.Zoomable, true)
            .Add(x => x.Zoom, new ChartZoom(1, 2))
            .Add(x => x.ZoomChanged, (ChartZoom? z) => reported = z));

        cut.FindAll($".{Css.Classes.Chart.Toolbar} button")[2].Click();

        Assert.Null(reported);
    }

    [Fact]
    public void AxisLabelDensity_FollowsTheAvailableWidth()
    {
        var data = new ChartData(
            [new ChartSeries("A", [1, 2, 3, 4, 5, 6, 7, 8])],
            ["a", "b", "c", "d", "e", "f", "g", "h"]);

        int LabelCount(int width)
        {
            var cut = Render<FlareChart>(p => p
                .Add(x => x.Type, ChartType.Line)
                .Add(x => x.Data, data)
                .Add(x => x.Fluid, false)
                .Add(x => x.Width, width));
            return cut.FindAll("text").Count(t => "abcdefgh".Contains(t.TextContent.Trim())
                                                  && t.TextContent.Trim().Length == 1);
        }

        // A wide chart has room for every category; a narrow one thins them out instead of overlapping.
        Assert.Equal(8, LabelCount(900));
        Assert.True(LabelCount(240) < 8);
    }

    [Fact]
    public void AxisLabels_FollowTheVisibleWindow()
    {
        var data = new ChartData(
            [new ChartSeries("A", [1, 2, 3, 4, 5, 6, 7, 8])],
            ["a", "b", "c", "d", "e", "f", "g", "h"]);

        var zoomed = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, data)
            .Add(x => x.Zoomable, true)
            .Add(x => x.Zoom, new ChartZoom(0, 2)));

        var labels = zoomed.FindAll("text").Select(t => t.TextContent).ToList();
        Assert.Contains("a", labels);
        Assert.Contains("b", labels);
        Assert.Contains("c", labels);
        Assert.DoesNotContain("h", labels);
    }
}
