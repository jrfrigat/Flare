namespace Flare.Components.Tests;

public class ChartPointBoundaryTests : FlareTestContext
{
    private static ChartData Points(params ChartPoint[] points) => new([new ChartSeries("Points", [], Points: points)]);

    [Theory]
    [InlineData(ChartType.Scatter)]
    [InlineData(ChartType.Bubble)]
    public void InvalidCoordinatesDoNotPoisonValidPointsOrTrendline(ChartType type)
    {
        var valid = new[] { new ChartPoint(1, 2, 1), new ChartPoint(3, 4, 4) };
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.TrendLine, true).Add(x => x.Data, Points(valid)));
        var marks = cut.FindAll("svg circle:not([fill='transparent']),svg path").Select(x => x.OuterHtml).ToArray();
        cut.Render(p => p.Add(x => x.Data, Points([.. valid, new(double.NaN, 3), new(4, double.PositiveInfinity), new(double.NegativeInfinity, 2)])));
        Assert.Equal(marks, cut.FindAll("svg circle:not([fill='transparent']),svg path").Select(x => x.OuterHtml).ToArray());
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(double.NaN)]
    [InlineData(double.PositiveInfinity)]
    [InlineData(double.NegativeInfinity)]
    public void InvalidBubbleWeightUsesTheZeroWeightRadius(double weight)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Bubble)
            .Add(x => x.Data, Points(new(1, 2, 0), new(3, 4, 4))));
        var radii = cut.FindAll("svg circle:not([fill='transparent'])").Select(x => x.GetAttribute("r")).ToArray();
        cut.Render(p => p.Add(x => x.Data, Points(new(1, 2, weight), new(3, 4, 4))));
        Assert.Equal(radii, cut.FindAll("svg circle:not([fill='transparent'])").Select(x => x.GetAttribute("r")).ToArray());
    }

    [Theory]
    [InlineData(ChartType.Scatter)]
    [InlineData(ChartType.Bubble)]
    public void AllInvalidPointsProduceNoMarks(ChartType type)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.TrendLine, true)
            .Add(x => x.Data, Points(new(double.NaN, 1), new(2, double.NegativeInfinity))));
        Assert.Empty(cut.FindAll("svg circle,svg path"));
        Assert.DoesNotContain("NaN", cut.Find("svg").OuterHtml);
        Assert.DoesNotContain("Infinity", cut.Find("svg").OuterHtml);
    }

    [Theory]
    [InlineData(ChartType.Scatter, ChartScaleMode.FitVisible)]
    [InlineData(ChartType.Scatter, ChartScaleMode.FitAll)]
    [InlineData(ChartType.Bubble, ChartScaleMode.FitVisible)]
    [InlineData(ChartType.Bubble, ChartScaleMode.FitAll)]
    public void HiddenPointsFollowTheSelectedScalePolicy(ChartType type, ChartScaleMode mode)
    {
        var first = new ChartSeries("First", [], Points: [new(1, 2, 1), new(3, 4, 4)]);
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.ScaleMode, mode)
            .Add(x => x.Data, new ChartData([first, new("Extreme", [], Points: [new(100, 200, 400)])])));
        var before = cut.FindAll("svg circle:not([fill='transparent'])").Take(2).Select(x => x.OuterHtml).ToArray();
        cut.FindAll($".{Css.Classes.Chart.LegendItem}")[1].Click();
        var hidden = cut.FindAll("svg circle:not([fill='transparent'])").Select(x => x.OuterHtml).ToArray();
        if (mode == ChartScaleMode.FitAll) Assert.Equal(before, hidden);
        else
        {
            Assert.NotEqual(string.Join("|", before), string.Join("|", hidden));
            cut.Render(p => p.Add(x => x.Data, new ChartData([first])));
            Assert.Equal(hidden, cut.FindAll("svg circle:not([fill='transparent'])").Select(x => x.OuterHtml).ToArray());
        }
    }

    [Fact]
    public void BubbleTableKeepsTheOriginalWeightAndMissingCoordinates()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Bubble).Add(x => x.DataTable, true)
            .Add(x => x.Data, Points(new(1, 2, -4), new(double.NaN, 3, 5))));
        Assert.Single(cut.FindAll("svg circle:not([fill='transparent'])"));
        Assert.Equal(2, cut.FindAll("tbody tr").Count);
        Assert.Equal("-4", cut.FindAll("tbody tr")[0].LastElementChild!.TextContent);
        Assert.Contains("No value", cut.FindAll("tbody tr")[1].TextContent);
    }

    [Theory]
    [InlineData(ChartScaleMode.FitVisible)]
    [InlineData(ChartScaleMode.FitAll)]
    public void HeatMapHiddenRowsFollowTheScalePolicy(ChartScaleMode mode)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Line).Add(x => x.ScaleMode, mode)
            .Add(x => x.Data, new ChartData([new("First", [0, 2]), new("Extreme", [100, 200])])));
        cut.Render(p => p.Add(x => x.Type, ChartType.HeatMap));
        var before = cut.FindAll($".{Css.Classes.Chart.Cell}").Take(2).Select(x => x.GetAttribute("style")).ToArray();
        cut.Render(p => p.Add(x => x.Type, ChartType.Line));
        cut.FindAll($".{Css.Classes.Chart.LegendItem}")[1].Click();
        cut.Render(p => p.Add(x => x.Type, ChartType.HeatMap));
        var hidden = cut.FindAll($".{Css.Classes.Chart.Cell}").Select(x => x.GetAttribute("style")).ToArray();
        Assert.Equal(2, hidden.Length);
        if (mode == ChartScaleMode.FitAll) Assert.Equal(before, hidden);
        else Assert.NotEqual(string.Join("|", before), string.Join("|", hidden));
    }

    [Fact]
    public void HeatMapMissingSlotsAreNotZeroCells()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.HeatMap).Add(x => x.ShowValues, true)
            .Add(x => x.Data, new ChartData([new("First", [0, double.NaN, 2, double.PositiveInfinity]), new("Short", [1])])));
        Assert.Equal(3, cut.FindAll($".{Css.Classes.Chart.Cell}").Count);
        Assert.Equal(new[] { "0", "2", "1" }, cut.FindAll("svg text[text-anchor='middle']").Select(x => x.TextContent).ToArray());
    }
}
