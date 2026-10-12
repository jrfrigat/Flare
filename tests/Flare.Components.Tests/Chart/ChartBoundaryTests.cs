namespace Flare.Components.Tests;

public class ChartBoundaryTests : FlareTestContext
{
    private static ChartData Data(double middle) => new([new ChartSeries("Series", [1, middle, 3])], ["A", "B", "C"]);

    private static string Geometry(IRenderedComponent<FlareChart> cut) => string.Join("|",
        cut.FindAll("svg path,svg rect,svg circle,svg line,svg text,svg polygon").Select(el =>
            el.TagName + ":" + string.Join(",", new[] { "d", "points", "x", "y", "cx", "cy", "r", "x1", "x2", "y1", "y2", "width", "height" }
                .Select(el.GetAttribute)) + ":" + el.TextContent));

    [Theory]
    [InlineData(ChartType.Line)]
    [InlineData(ChartType.Area)]
    [InlineData(ChartType.Bar)]
    [InlineData(ChartType.Combo)]
    public void FiniteNegativeValuesRemainBelowZero(ChartType type)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Data(-2)));
        Assert.Contains(cut.FindAll("svg text"), x => x.TextContent.StartsWith('-'));
        var negative = Geometry(cut);
        cut.Render(p => p.Add(x => x.Data, Data(0)));
        Assert.NotEqual(negative, Geometry(cut));
    }

    [Theory]
    [InlineData(ChartType.Line)]
    [InlineData(ChartType.Area)]
    [InlineData(ChartType.Bar)]
    [InlineData(ChartType.StackedBar)]
    [InlineData(ChartType.Combo)]
    public void AnEntirelyMissingSeriesNeverProducesNonfiniteSvg(ChartType type)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type)
            .Add(x => x.Data, new ChartData([new ChartSeries("Series", [double.NaN, double.PositiveInfinity, double.NegativeInfinity])])));
        Assert.DoesNotContain("NaN", cut.Find("svg").OuterHtml);
        Assert.DoesNotContain("Infinity", cut.Find("svg").OuterHtml);
    }

    [Fact]
    public void AValidUpperBoundSurvivesAnInvalidLowerBound()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data(2)).Add(x => x.YMax, 10));
        var pinned = Geometry(cut);
        cut.Render(p => p.Add(x => x.YMin, double.NaN));
        Assert.Equal(pinned, Geometry(cut));
    }

    [Fact]
    public void AConstantSeriesKeepsItsOneUnitRange()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.NiceScale, false).Add(x => x.YAxisTickCount, 2)
            .Add(x => x.Data, new ChartData([new ChartSeries("Series", [1000, 1000])])));
        Assert.Equal(new[] { "1000", "1001" }, cut.FindAll("svg text").Select(x => x.TextContent).ToArray());
    }

    [Theory]
    [InlineData(ChartType.Line)]
    [InlineData(ChartType.Area)]
    [InlineData(ChartType.Bar)]
    [InlineData(ChartType.StackedBar)]
    [InlineData(ChartType.Combo)]
    public void InfinityBehavesAsAMissingCategoryValue(ChartType type)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Data(double.NaN)));
        var missing = Geometry(cut);
        foreach (var value in new[] { double.PositiveInfinity, double.NegativeInfinity })
        {
            cut.Render(p => p.Add(x => x.Data, Data(value)));
            Assert.Equal(missing, Geometry(cut));
            Assert.DoesNotContain("NaN", cut.Find("svg").OuterHtml);
            Assert.DoesNotContain("Infinity", cut.Find("svg").OuterHtml);
        }
    }

    [Theory]
    [InlineData(double.NaN, double.NaN)]
    [InlineData(double.NegativeInfinity, double.PositiveInfinity)]
    [InlineData(10, 0)]
    [InlineData(2, 2)]
    public void InvalidAxisBoundsUseTheAutomaticRange(double min, double max)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data(2)));
        var automatic = Geometry(cut);
        cut.Render(p => p.Add(x => x.YMin, min).Add(x => x.YMax, max));
        Assert.Equal(automatic, Geometry(cut));
    }

    [Theory]
    [InlineData(ChartType.StackedBar, 10, 0)]
    [InlineData(ChartType.StackedBar, 2, 2)]
    [InlineData(ChartType.StackedBar, double.NaN, 0)]
    [InlineData(ChartType.StackedBar, double.NaN, -2)]
    public void InvalidStackBoundsUseThePositiveAutomaticRange(ChartType type, double min, double max)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Data(2)));
        var automatic = Geometry(cut);
        cut.Render(p => p.Add(x => x.YMin, min).Add(x => x.YMax, max));
        Assert.Equal(automatic, Geometry(cut));
        Assert.DoesNotContain("Infinity", cut.Find("svg").OuterHtml);
    }

    [Theory]
    [InlineData(0, 0, 0, false)]
    [InlineData(-10, -10, -10, false)]
    [InlineData(10, 10, 500, false)]
    [InlineData(1, 1, 500, true)]
    public void SmallAndInvalidDimensionsKeepPositivePlotSizes(int width, int height, int padding, bool sparkline)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Bar).Add(x => x.Data, Data(2))
            .Add(x => x.Width, width).Add(x => x.Height, height).Add(x => x.Padding, padding)
            .Add(x => x.Sparkline, sparkline));
        var viewBox = cut.Find("svg").GetAttribute("viewBox")!.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        Assert.True(double.Parse(viewBox[2], System.Globalization.CultureInfo.InvariantCulture) > 0);
        Assert.True(double.Parse(viewBox[3], System.Globalization.CultureInfo.InvariantCulture) > 0);
        foreach (var el in cut.FindAll("svg [width],svg [height],svg [r]"))
            foreach (var attribute in new[] { "width", "height", "r" })
                if (el.GetAttribute(attribute) is { } number)
                    Assert.True(double.Parse(number, System.Globalization.CultureInfo.InvariantCulture) >= 0, attribute + "=" + number);
    }
}
