using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class ChartVisibilityTests : FlareTestContext
{
    private static readonly ChartData Data = new(
        [new ChartSeries("First", [10, 20, 30]), new ChartSeries("Second", [30, 40, 50])],
        ["A", "B", "C"]);

    private IRenderedComponent<FlareChart> Chart(ChartType type) => Render<FlareChart>(p => p
        .Add(x => x.Type, type).Add(x => x.Data, Data).Add(x => x.ShowValues, true));

    private static void Toggle(IRenderedComponent<FlareChart> cut, int index) =>
        cut.FindAll($".{Css.Classes.Chart.LegendItem}")[index].Click();

    [Theory]
    [InlineData(ChartType.Pie)]
    [InlineData(ChartType.Donut)]
    [InlineData(ChartType.Rose)]
    [InlineData(ChartType.PolarArea)]
    public void RadialMarksDisappearAndRestoreWithTheirSeries(ChartType type)
    {
        var cut = Chart(type);
        const string marks = "svg path:not([fill='transparent'])";
        var before = cut.FindAll(marks).Select(x => x.OuterHtml).ToArray();
        var secondStyle = cut.FindAll(marks)[1].GetAttribute("style");
        Assert.Equal(2, before.Length);
        Toggle(cut, 1);
        Assert.Single(cut.FindAll(marks));
        Toggle(cut, 0);
        Assert.Empty(cut.FindAll(marks));
        Assert.Empty(cut.FindAll("svg path[fill='transparent']"));
        Toggle(cut, 1);
        Assert.Equal(secondStyle, cut.FindAll(marks).Single().GetAttribute("style"));
        Toggle(cut, 1);
        Toggle(cut, 0);
        Toggle(cut, 1);
        Assert.Equal(before, cut.FindAll(marks).Select(x => x.OuterHtml).ToArray());
    }

    [Theory]
    [InlineData(ChartType.Pie)]
    [InlineData(ChartType.Donut)]
    public void SliceLabelsHitZonesAndTooltipUseVisibleTotal(ChartType type)
    {
        var cut = Chart(type);
        Toggle(cut, 1);
        Assert.Single(cut.FindAll("svg path[fill='transparent']"));
        Assert.Contains(cut.FindAll("svg text"), x => x.TextContent == "100%");
        cut.Find("svg path[fill='transparent']").MouseEnter(new MouseEventArgs { ClientX = 100, ClientY = 100 });
        Assert.Equal("First: 100.0%", cut.Find($".{Css.Classes.Chart.Tooltip}").TextContent.Trim());
    }

    [Fact]
    public void CategoryTooltipOmitsHiddenSeriesAndClosesWhenLegendChanges()
    {
        var cut = Chart(ChartType.Line);
        Toggle(cut, 1);
        cut.Find("svg rect[fill='transparent']").MouseEnter(new MouseEventArgs());
        var text = cut.Find($".{Css.Classes.Chart.Tooltip}").TextContent;
        Assert.Contains("First: 10", text);
        Assert.DoesNotContain("Second", text);
        Toggle(cut, 0);
        Assert.Empty(cut.FindAll($".{Css.Classes.Chart.Tooltip}"));
        cut.Find("svg rect[fill='transparent']").MouseEnter(new MouseEventArgs());
        Assert.Empty(cut.FindAll($".{Css.Classes.Chart.Tooltip}"));
    }

    [Fact]
    public void HeatMapRowsDisappearAndRestoreWithTheirSeries()
    {
        var cut = Chart(ChartType.Line);
        cut.Render(p => p.Add(x => x.Type, ChartType.HeatMap));
        var before = cut.FindAll($".{Css.Classes.Chart.Cell}").Select(x => x.OuterHtml).ToArray();
        Assert.Equal(6, before.Length);
        cut.Render(p => p.Add(x => x.Type, ChartType.Line));
        Toggle(cut, 1);
        cut.Render(p => p.Add(x => x.Type, ChartType.HeatMap));
        Assert.Equal(3, cut.FindAll($".{Css.Classes.Chart.Cell}").Count);
        Assert.DoesNotContain(cut.FindAll("svg text"), x => x.TextContent == "Second");
        cut.Render(p => p.Add(x => x.Type, ChartType.Line));
        Toggle(cut, 0);
        cut.Render(p => p.Add(x => x.Type, ChartType.HeatMap));
        Assert.Empty(cut.FindAll($".{Css.Classes.Chart.Cell}"));
        cut.Render(p => p.Add(x => x.Type, ChartType.Line));
        Toggle(cut, 0);
        Toggle(cut, 1);
        cut.Render(p => p.Add(x => x.Type, ChartType.HeatMap));
        Assert.Equal(before, cut.FindAll($".{Css.Classes.Chart.Cell}").Select(x => x.OuterHtml).ToArray());
    }
}
