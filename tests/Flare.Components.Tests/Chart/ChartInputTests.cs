using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class ChartInputTests : FlareTestContext
{
    private static readonly ChartData Data = new([new("First", [1, double.NaN, 3]), new("Second", [2, double.NaN, 4])], ["A", "B", "C"]);

    [Theory]
    [InlineData(ChartType.Line)]
    [InlineData(ChartType.Area)]
    [InlineData(ChartType.Bar)]
    [InlineData(ChartType.StackedBar)]
    [InlineData(ChartType.Combo)]
    [InlineData(ChartType.Pie)]
    [InlineData(ChartType.Donut)]
    public void HitsAreLabelledOperableAndHaveOneTabEntry(ChartType type)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Data));
        Assert.Equal("group", cut.Find("svg").GetAttribute("role"));
        var hits = cut.FindAll("svg [data-flare-chart-hit]");
        Assert.Equal(2, hits.Count);
        Assert.Single(hits, x => x.GetAttribute("tabindex") == "0");
        Assert.All(hits, hit => { Assert.Equal("button", hit.GetAttribute("role")); Assert.False(string.IsNullOrWhiteSpace(hit.GetAttribute("aria-label"))); });
    }

    [Theory]
    [InlineData(ChartType.Line)]
    [InlineData(ChartType.Pie)]
    public void ClickShowsDetailsAndPreservesCallbackIndex(ChartType type)
    {
        int? index = null;
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Data)
            .Add(x => x.OnPointClick, (int value) => index = value));
        var selector = type == ChartType.Line ? "svg rect[fill='transparent']" : "svg path[fill='transparent']";
        cut.Find(selector).Click(new MouseEventArgs { ClientX = 120, ClientY = 150 });
        Assert.Equal(0, index);
        Assert.Contains("First", cut.Find("[role=tooltip]").TextContent);
        Assert.Equal(cut.Find("[role=tooltip]").Id, cut.Find("[data-flare-chart-tooltip]").GetAttribute("data-flare-chart-tooltip"));
        Assert.Equal(type == ChartType.Line ? "category:0" : "slice:0",
            cut.Find("[data-flare-chart-active]").GetAttribute("data-flare-chart-active"));
    }

    [Fact]
    public void ClickWithoutCallbackStillShowsValues()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data));
        cut.Find("svg rect[fill='transparent']").Click(new MouseEventArgs { ClientX = 100, ClientY = 100 });
        Assert.Contains("First: 1", cut.Find("[role=tooltip]").TextContent);
    }

    [Fact]
    public void HorizontalBarHitZonesFollowCategoryRows()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Bar).Add(x => x.Horizontal, true).Add(x => x.Data, Data));
        var hits = cut.FindAll("svg rect[fill='transparent']");
        Assert.Equal(hits[0].GetAttribute("x"), hits[^1].GetAttribute("x"));
        Assert.NotEqual(hits[0].GetAttribute("y"), hits[^1].GetAttribute("y"));
    }

    [Fact]
    public async Task FocusKeepsDetailsAfterPointerLeavesAndEscapeDismisses()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data));
        await cut.Instance.ChartHitFocused("category:0", 100, 100);
        cut.Find("svg rect[fill='transparent']").MouseLeave();
        Assert.Contains("First: 1", cut.Find("[role=tooltip]").TextContent);
        await cut.Instance.ChartDetailDismissed();
        Assert.Empty(cut.FindAll("[role=tooltip]"));
    }

    [Fact]
    public async Task RemovedDataDismissesFocusedDetails()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data));
        await cut.Instance.ChartHitFocused("category:0", 100, 100);
        cut.Render(p => p.Add(x => x.Data, new ChartData([])));
        Assert.Empty(cut.FindAll("[role=tooltip]"));
    }

    [Fact]
    public void HiddenAndSparklineDataHaveNoOperableHits()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Data, Data));
        foreach (var index in new[] { 0, 1 }) cut.FindAll($".{Css.Classes.Chart.LegendItem}")[index].Click();
        Assert.Empty(cut.FindAll("svg [data-flare-chart-hit]"));
        cut.Render(p => p.Add(x => x.Sparkline, true));
        Assert.Empty(cut.FindAll("svg [data-flare-chart-hit]"));
    }
}
