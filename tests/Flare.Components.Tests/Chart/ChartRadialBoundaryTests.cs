namespace Flare.Components.Tests;

public class ChartRadialBoundaryTests : FlareTestContext
{
    private static ChartData Data(params double[] values) => new(values.Select((v, i) => new ChartSeries("Series" + i, [v])).ToArray());
    private IRenderedComponent<FlareChart> Chart(ChartType type, params double[] values) =>
        Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Data(values)).Add(x => x.ShowValues, true));
    private static string[] Paths(IRenderedComponent<FlareChart> cut) =>
        cut.FindAll("svg path:not([fill='transparent'])").Select(x => x.GetAttribute("d")!).ToArray();

    [Theory]
    [InlineData(ChartType.Pie, 2)]
    [InlineData(ChartType.Donut, 4)]
    [InlineData(ChartType.Rose, 2)]
    [InlineData(ChartType.PolarArea, 2)]
    public void OneVisibleSliceHasTwoArcsPerCircularContour(ChartType type, int arcs)
    {
        var cut = Chart(type, 1, 2);
        cut.FindAll($".{Css.Classes.Chart.LegendItem}")[1].Click();
        Assert.Equal(arcs, System.Text.RegularExpressions.Regex.Matches(Assert.Single(Paths(cut)), " A ").Count);
    }

    [Theory]
    [InlineData(ChartType.Pie)]
    [InlineData(ChartType.Donut)]
    [InlineData(ChartType.Rose)]
    [InlineData(ChartType.PolarArea)]
    public void InvalidAndZeroWeightsDoNotCreateSectors(ChartType type)
    {
        var cut = Chart(type, 0, -1, double.NaN, double.PositiveInfinity, double.NegativeInfinity);
        Assert.Empty(Paths(cut));
        Assert.Empty(cut.FindAll("svg path[fill='transparent']"));
        Assert.DoesNotContain("NaN", cut.Find("svg").OuterHtml);
    }

    [Theory]
    [InlineData(ChartType.Pie)]
    [InlineData(ChartType.Donut)]
    [InlineData(ChartType.Rose)]
    [InlineData(ChartType.PolarArea)]
    public void InvalidWeightsDoNotChangeTheValidSectors(ChartType type)
    {
        var cut = Chart(type, 1, 2);
        var before = Paths(cut);
        cut.Render(p => p.Add(x => x.Data, Data(1, 2, -4, double.NaN, double.PositiveInfinity, 0)));
        Assert.Equal(before, Paths(cut));
    }

    [Theory]
    [InlineData(ChartType.Pie)]
    [InlineData(ChartType.Donut)]
    [InlineData(ChartType.Rose)]
    [InlineData(ChartType.PolarArea)]
    public void TinyPlotsDoNotDrawNegativeRadiusPaths(ChartType type)
    {
        var cut = Chart(type, 1, 2);
        cut.Render(p => p.Add(x => x.Width, 1).Add(x => x.Height, 1));
        Assert.Empty(Paths(cut));
        foreach (var circle in cut.FindAll("svg circle"))
            Assert.True(double.Parse(circle.GetAttribute("r")!, System.Globalization.CultureInfo.InvariantCulture) >= 0);
    }

    [Fact]
    public void RadarNegativeAndMissingWeightsStayAtTheCenter()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Radar)
            .Add(x => x.Data, new ChartData([new("Radar", [0, 1, 2])])));
        var before = cut.FindAll("svg polygon").Select(x => x.GetAttribute("points")).ToArray();
        foreach (var value in new[] { -2d, double.NaN, double.PositiveInfinity })
        {
            cut.Render(p => p.Add(x => x.Data, new ChartData([new("Radar", [value, 1, 2])])));
            Assert.Equal(before, cut.FindAll("svg polygon").Select(x => x.GetAttribute("points")).ToArray());
        }
    }

    [Theory]
    [InlineData(ChartType.Pie)]
    [InlineData(ChartType.Donut)]
    public void LargeFiniteWeightsKeepTheirProportions(ChartType type)
    {
        var cut = Chart(type, 1, 1);
        var before = Paths(cut);
        cut.Render(p => p.Add(x => x.Data, Data(1e308, 1e308)));
        Assert.Equal(before, Paths(cut));
    }
}
