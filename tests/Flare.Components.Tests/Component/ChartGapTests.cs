using System.Text.RegularExpressions;

namespace Flare.Components.Tests.Component;

/// <summary>
/// Two things a chart has to be able to say that it could not before: "this series has no value here"
/// and "this overlay belongs behind the data". Both used to have only wrong answers - a zero, which is
/// a value and reads as one, and a band painted over the numbers it was drawn to frame.
/// </summary>
public sealed class ChartGapTests : FlareTestContext
{
    private static string Paths(IRenderedComponent<FlareChart> cut) => cut.Markup;

    private IRenderedComponent<FlareChart> RenderLine(ChartData data) =>
        Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, data));

    private IRenderedComponent<FlareChart> RenderLine(ChartData data, IReadOnlyList<ChartAnnotation> annotations) =>
        Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Line)
            .Add(x => x.Data, data)
            .Add(x => x.Annotations, annotations));

    // The path of the series line, as the `d` attribute the renderer produced.
    private static string LinePath(IRenderedComponent<FlareChart> cut) =>
        cut.Find($"path.{Css.Classes.Chart.Line}").GetAttribute("d") ?? "";

    [Fact]
    public void Gaps_ConvertsNullsToTheGapMarker()
    {
        var values = ChartSeries.Gaps([null, 1.0, null]);

        Assert.True(double.IsNaN(values[0]));
        Assert.Equal(1.0, values[1]);
        Assert.True(double.IsNaN(values[2]));
    }

    [Fact]
    public void Line_BreaksAtAGapInsteadOfCrossingIt()
    {
        var data = new ChartData(
            [new ChartSeries("Seasonal", ChartSeries.Gaps([1.0, 2.0, null, 4.0, 5.0]))],
            ["a", "b", "c", "d", "e"]);

        var d = LinePath(RenderLine(data));

        // Two subpaths: each run of present points starts its own M, so nothing is stroked over the hole.
        Assert.Equal(2, Regex.Matches(d, "M ").Count);
    }

    [Fact]
    public void Line_WithNoGaps_IsStillOneSubpath()
    {
        var data = new ChartData([new ChartSeries("Full", [1, 2, 3, 4])], ["a", "b", "c", "d"]);

        var subpaths = Regex.Matches(LinePath(RenderLine(data)), "M ").Count;
        Assert.Equal(1, subpaths);
    }

    // The point of a gap over a zero: a zero is a value, and it drags the axis down to it.
    [Fact]
    public void Gaps_DoNotStretchTheValueAxis()
    {
        var withGaps = new ChartData(
            [new ChartSeries("Seasonal", ChartSeries.Gaps([null, 100.0, 110.0, null]))], ["a", "b", "c", "d"]);
        var withZeros = new ChartData(
            [new ChartSeries("Seasonal", [0, 100, 110, 0])], ["a", "b", "c", "d"]);

        var gapY = LinePath(RenderLine(withGaps));
        var zeroY = LinePath(RenderLine(withZeros));

        Assert.NotEqual(zeroY, gapY);
        // A padded series has to reach the axis somewhere; a gapped one never does.
        Assert.Contains("0", zeroY, StringComparison.Ordinal);
    }

    [Fact]
    public void LoneValueBetweenGaps_IsStillDrawn()
    {
        var data = new ChartData(
            [new ChartSeries("One month", ChartSeries.Gaps([null, null, 7.0, null, null]))],
            ["a", "b", "c", "d", "e"]);

        // A run of one point has no segment, so without a dot the value would be in the data and
        // nowhere on the chart.
        Assert.NotEmpty(RenderLine(data).FindAll("circle"));
    }

    [Fact]
    public void Bar_DrawsNothingForAGap()
    {
        var full = new ChartData([new ChartSeries("S", [1, 2, 3])], ["a", "b", "c"]);
        var gapped = new ChartData([new ChartSeries("S", ChartSeries.Gaps([1.0, null, 3.0]))], ["a", "b", "c"]);

        int Bars(ChartData d) => Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Bar).Add(x => x.Data, d)).FindAll($"rect.{Css.Classes.Chart.Bar}").Count;

        Assert.Equal(3, Bars(full));
        Assert.Equal(2, Bars(gapped));
    }

    [Fact]
    public void Annotation_DefaultsToDrawingOverTheSeries()
    {
        var data = new ChartData([new ChartSeries("S", [1, 2, 3])], ["a", "b", "c"]);
        var markup = Paths(RenderLine(data, [ChartAnnotation.VerticalBand(0, 1, "plan")]));

        Assert.True(markup.IndexOf(Css.Classes.Chart.Line, StringComparison.Ordinal)
            < markup.IndexOf("plan", StringComparison.Ordinal),
            "An annotation with no layer stated keeps the behaviour it always had: over the data.");
    }

    [Fact]
    public void Annotation_Under_DrawsBeforeTheSeries()
    {
        var data = new ChartData([new ChartSeries("S", [1, 2, 3])], ["a", "b", "c"]);
        var markup = Paths(RenderLine(data,
            [ChartAnnotation.VerticalBand(0, 1, "plan", layer: ChartAnnotationLayer.Under)]));

        Assert.True(markup.IndexOf("plan", StringComparison.Ordinal)
            < markup.IndexOf(Css.Classes.Chart.Line, StringComparison.Ordinal),
            "An Under band paints first, so the series draws on top of it instead of through it.");
    }

    [Fact]
    public void Annotations_SplitAcrossBothLayers_KeepTheirOrder()
    {
        var data = new ChartData([new ChartSeries("S", [1, 2, 3])], ["a", "b", "c"]);
        var markup = Paths(RenderLine(data,
        [
            ChartAnnotation.VerticalBand(0, 1, "behind", layer: ChartAnnotationLayer.Under),
            ChartAnnotation.Threshold(2, "in front"),
        ]));

        int behind = markup.IndexOf("behind", StringComparison.Ordinal);
        int line = markup.IndexOf(Css.Classes.Chart.Line, StringComparison.Ordinal);
        int front = markup.IndexOf("in front", StringComparison.Ordinal);

        Assert.True(behind < line && line < front);
    }
}
