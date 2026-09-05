namespace Flare.Components.Tests.Component;

/// <summary>
/// What a click on the legend is allowed to change. The reader who switches a series off is asking to
/// see the rest without it, and the default answer moved everything else instead: the axis was measured
/// from the visible series, so the window, the grid lines and every remaining mark shifted.
/// <see cref="ChartScaleMode.FitAll"/> is the other answer - the series disappears and nothing moves.
/// </summary>
public sealed class ChartScaleModeTests : FlareTestContext
{
    // Two series with clearly different tops: hiding the taller one is what shrinks the domain.
    private static readonly ChartData Data = new(
        [
            new ChartSeries("Coffee", [42, 38, 45, 51]),
            new ChartSeries("Ice cream", [10, 20, 90, 15]),
        ],
        ["a", "b", "c", "d"]);

    private IRenderedComponent<FlareChart> RenderChart(ChartType type, ChartScaleMode mode) =>
        Render<FlareChart>(p => p
            .Add(x => x.Type, type)
            .Add(x => x.Data, Data)
            .Add(x => x.ScaleMode, mode));

    // The legend is the only way in, so the test goes through it rather than through internal state.
    private static void HideSecondSeries(IRenderedComponent<FlareChart> cut) =>
        cut.FindAll($".{Css.Classes.Chart.LegendItem}")[1].Click();

    private static string GridLines(IRenderedComponent<FlareChart> cut) =>
        string.Join("|", cut.FindAll("text").Select(t => t.TextContent.Trim()));

    private static string FirstLinePath(IRenderedComponent<FlareChart> cut) =>
        cut.FindAll($"path.{Css.Classes.Chart.Line}")[0].GetAttribute("d") ?? "";

    [Fact]
    public void FitVisible_IsTheDefault()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Line).Add(x => x.Data, Data));
        var before = FirstLinePath(cut);

        HideSecondSeries(cut);

        Assert.NotEqual(before, FirstLinePath(cut));
    }

    [Fact]
    public void FitAll_LeavesTheRemainingLineExactlyWhereItWas()
    {
        var cut = RenderChart(ChartType.Line, ChartScaleMode.FitAll);
        var before = FirstLinePath(cut);
        var axisBefore = GridLines(cut);

        HideSecondSeries(cut);

        Assert.Equal(before, FirstLinePath(cut));
        Assert.Equal(axisBefore, GridLines(cut));
    }

    [Fact]
    public void FitAll_StillRemovesTheHiddenSeries()
    {
        var cut = RenderChart(ChartType.Line, ChartScaleMode.FitAll);
        Assert.Equal(2, cut.FindAll($"path.{Css.Classes.Chart.Line}").Count);

        HideSecondSeries(cut);

        Assert.Single(cut.FindAll($"path.{Css.Classes.Chart.Line}"));
        Assert.Contains(Css.Classes.Chart.LegendItemOff,
            cut.FindAll($".{Css.Classes.Chart.LegendItem}")[1].ClassName, StringComparison.Ordinal);
    }

    [Fact]
    public void FitVisible_RepacksTheBarGroup()
    {
        var cut = RenderChart(ChartType.Bar, ChartScaleMode.FitVisible);
        var before = cut.FindAll($"rect.{Css.Classes.Chart.Bar}")[0].GetAttribute("width");

        HideSecondSeries(cut);

        // One series left in a group sized for one: the bars are wider than they were beside a neighbour.
        Assert.NotEqual(before, cut.FindAll($"rect.{Css.Classes.Chart.Bar}")[0].GetAttribute("width"));
    }

    [Fact]
    public void FitAll_KeepsEveryBarWhereItWas()
    {
        var cut = RenderChart(ChartType.Bar, ChartScaleMode.FitAll);
        var before = cut.FindAll($"rect.{Css.Classes.Chart.Bar}")
            .Take(4)
            .Select(r => (r.GetAttribute("x"), r.GetAttribute("y"), r.GetAttribute("width")))
            .ToList();

        HideSecondSeries(cut);

        var after = cut.FindAll($"rect.{Css.Classes.Chart.Bar}")
            .Select(r => (r.GetAttribute("x"), r.GetAttribute("y"), r.GetAttribute("width")))
            .ToList();
        Assert.Equal(4, after.Count);
        Assert.Equal(before, after);
    }

    // The combo renderer had the opposite behaviour to the grouped bar chart before the option existed:
    // it re-packed while the bar chart left a hole. Both now follow the mode.
    [Fact]
    public void Combo_FollowsTheSameRuleAsTheBarChart()
    {
        var comboData = new ChartData(
            [
                new ChartSeries("Coffee", [42, 38, 45, 51], Kind: ChartSeriesKind.Bar),
                new ChartSeries("Ice cream", [10, 20, 90, 15], Kind: ChartSeriesKind.Bar),
            ],
            ["a", "b", "c", "d"]);

        IRenderedComponent<FlareChart> Combo(ChartScaleMode mode) => Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Combo)
            .Add(x => x.Data, comboData)
            .Add(x => x.ScaleMode, mode));

        var held = Combo(ChartScaleMode.FitAll);
        var heldBefore = held.FindAll($"rect.{Css.Classes.Chart.Bar}").Take(4).Select(r => r.GetAttribute("x")).ToList();
        HideSecondSeries(held);
        Assert.Equal(heldBefore, held.FindAll($"rect.{Css.Classes.Chart.Bar}").Select(r => r.GetAttribute("x")).ToList());

        var refit = Combo(ChartScaleMode.FitVisible);
        var refitBefore = refit.FindAll($"rect.{Css.Classes.Chart.Bar}")[0].GetAttribute("x");
        HideSecondSeries(refit);
        Assert.NotEqual(refitBefore, refit.FindAll($"rect.{Css.Classes.Chart.Bar}")[0].GetAttribute("x"));
    }

    // A stacked band that is switched off frees room in every stack. Under FitAll the axis keeps that
    // room rather than letting the remaining bands grow into it.
    [Fact]
    public void FitAll_KeepsTheStackedAxisAtTheFullTotal()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.StackedBar)
            .Add(x => x.Data, Data)
            .Add(x => x.ScaleMode, ChartScaleMode.FitAll));
        var axisBefore = GridLines(cut);
        var firstBefore = cut.FindAll($"rect.{Css.Classes.Chart.Bar}")[0].GetAttribute("height");

        HideSecondSeries(cut);

        Assert.Equal(axisBefore, GridLines(cut));
        Assert.Equal(firstBefore, cut.FindAll($"rect.{Css.Classes.Chart.Bar}")[0].GetAttribute("height"));
    }

    [Fact]
    public void FitAll_HoldsTheRadarWebToo()
    {
        var radarData = new ChartData(
            [
                new ChartSeries("Coffee", [42, 38, 45, 51]),
                new ChartSeries("Ice cream", [10, 20, 90, 15]),
            ],
            ["a", "b", "c", "d"]);

        var cut = Render<FlareChart>(p => p
            .Add(x => x.Type, ChartType.Radar)
            .Add(x => x.Data, radarData)
            .Add(x => x.ScaleMode, ChartScaleMode.FitAll));
        var before = cut.FindAll("polygon").Select(g => g.GetAttribute("points")).First();

        HideSecondSeries(cut);

        Assert.Equal(before, cut.FindAll("polygon").Select(g => g.GetAttribute("points")).First());
    }
}
