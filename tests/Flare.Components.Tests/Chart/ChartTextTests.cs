namespace Flare.Components.Tests;

public class ChartTextTests : FlareTestContext
{
    private const string Label = "A <tspan> & \"Q\"";

    [Theory]
    [InlineData(ChartType.Line, false)]
    [InlineData(ChartType.Bar, true)]
    [InlineData(ChartType.Radar, false)]
    [InlineData(ChartType.HeatMap, false)]
    public void DataLabelsRemainLiteralText(ChartType type, bool horizontal)
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, type)
            .Add(x => x.Horizontal, horizontal)
            .Add(x => x.Data, new ChartData([new ChartSeries(Label, [1, 2, 3])], [Label, "B", "C"])));

        Assert.Contains(cut.FindAll("svg text"), x => x.TextContent == Label);
        Assert.Empty(cut.FindAll("svg tspan"));
    }

    [Fact]
    public void AxisTitlesAndAnnotationsRemainLiteralText()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Data, new ChartData([new ChartSeries("Series", [1, 2, 3])]))
            .Add(x => x.XAxisTitle, Label)
            .Add(x => x.YAxisTitle, Label)
            .Add(x => x.Annotations, [ChartAnnotation.Threshold(2, Label)]));

        Assert.Equal(3, cut.FindAll("svg text").Count(x => x.TextContent == Label));
        Assert.Empty(cut.FindAll("svg tspan"));
    }

    [Theory]
    [InlineData(false)]
    [InlineData(true)]
    public void FormattedAxisLabelsRemainLiteralText(bool horizontal)
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Bar)
            .Add(x => x.Horizontal, horizontal)
            .Add(x => x.Data, new ChartData([new ChartSeries("Series", [1, 2, 3])]))
            .Add(x => x.YAxisFormat, "'A <tspan> & Q' 0"));

        Assert.Contains(cut.FindAll("svg text"), x => x.TextContent.StartsWith("A <tspan> & Q "));
        Assert.Empty(cut.FindAll("svg tspan"));
    }
}
