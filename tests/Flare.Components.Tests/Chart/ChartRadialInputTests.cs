using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class ChartRadialInputTests : FlareTestContext
{
    private static readonly ChartData Sectors = new([new("Zero", [0]), new("First", [3]), new("Missing", [double.NaN]), new("Last", [5])]);
    private static readonly ChartData Radar = new([new("First", [0, double.NaN, 3]), new("Second", [2, double.NaN, 4]), new("Short", [9])], ["A", "B", "C"]);

    [Theory]
    [InlineData(ChartType.Rose)]
    [InlineData(ChartType.PolarArea)]
    public void SectorHitsUseExactlyTheVisualPaths(ChartType type)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Sectors));
        var hits = cut.FindAll("path[data-flare-chart-hit]");Assert.Equal(2, hits.Count);
        var marks = cut.FindAll("path:not([fill='transparent'])");
        Assert.Equal(marks.Select(x => x.GetAttribute("d")), hits.Select(x => x.GetAttribute("d")));
        Assert.Single(hits, h => h.GetAttribute("tabindex") == "0");
    }

    [Theory]
    [InlineData(ChartType.Rose)]
    [InlineData(ChartType.PolarArea)]
    public void SectorActivationReportsOriginalSeriesAndValue(ChartType type)
    {
        int? selected = null;
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Sectors)
            .Add(x => x.OnPointClick, (int i) => selected = i));
        cut.Find("[data-flare-chart-hit='sector:3']").Click(new MouseEventArgs());
        Assert.Equal(3, selected);Assert.Equal("Last: 5", cut.Find("[role=tooltip]").TextContent.Trim());
    }

    [Theory]
    [InlineData(ChartType.Rose)]
    [InlineData(ChartType.PolarArea)]
    public void HiddenSectorLeavesOneFullCircleWithOriginalIndex(ChartType type)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Sectors));
        cut.FindAll($".{Css.Classes.Chart.LegendItem}")[3].Click();
        var hit = Assert.Single(cut.FindAll("[data-flare-chart-hit]"));
        Assert.Equal("sector:1", hit.GetAttribute("data-flare-chart-hit"));
        Assert.Equal(2, System.Text.RegularExpressions.Regex.Matches(hit.GetAttribute("d")!, " A ").Count);
    }

    [Theory]
    [InlineData(ChartType.Rose)]
    [InlineData(ChartType.PolarArea)]
    public void TinyAndInvalidSectorsHaveNoInputZones(ChartType type)
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, type).Add(x => x.Data, Sectors).Add(x => x.Width, 1).Add(x => x.Height, 1));
        Assert.Empty(cut.FindAll("[data-flare-chart-hit]"));
        cut.Render(p => p.Add(x => x.Width, 400).Add(x => x.Height, 220)
            .Add(x => x.Data, new ChartData([new("Missing", [double.PositiveInfinity]),new("Zero", [0])])));
        Assert.Empty(cut.FindAll("[data-flare-chart-hit]"));
    }

    [Fact]
    public void RadarCategoriesIncludeZeroOmitGapsAndUndrawnShortSeries()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Radar).Add(x => x.Data, Radar));
        var hits = cut.FindAll("[data-flare-chart-hit]");Assert.Equal(2, hits.Count);
        Assert.Contains("First: 0", hits[0].GetAttribute("aria-label"));
        Assert.DoesNotContain("Short", hits[0].GetAttribute("aria-label"));
        Assert.Equal("radar:2", hits[1].GetAttribute("data-flare-chart-hit"));
    }

    [Fact]
    public void RadarActivationReportsCategoryAndAllPresentSeries()
    {
        int? selected = null;
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Radar).Add(x => x.Data, Radar)
            .Add(x => x.OnPointClick, (int i) => selected = i));
        cut.Find("[data-flare-chart-hit='radar:2']").Click(new MouseEventArgs());
        Assert.Equal(2, selected);Assert.Contains("First: 3", cut.Find("[role=tooltip]").TextContent);Assert.Contains("Second: 4", cut.Find("[role=tooltip]").TextContent);
    }

    [Fact]
    public void RadarHiddenAndUnsupportedDataHaveNoInputZones()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Radar).Add(x => x.Data, Radar));
        foreach (int i in new[] { 0, 1, 2 }) cut.FindAll($".{Css.Classes.Chart.LegendItem}")[i].Click();
        Assert.Empty(cut.FindAll("[data-flare-chart-hit]"));
        cut.Render(p => p.Add(x => x.Data, new ChartData([new("Short", [1, 2])])));
        Assert.Empty(cut.FindAll("[data-flare-chart-hit]"));
    }

    [Fact]
    public void TinyRadarHasNoInvisibleFocusZones()
    {
        var cut = Render<FlareChart>(p => p.Add(x => x.Type, ChartType.Radar).Add(x => x.Data, Radar).Add(x => x.Width, 1).Add(x => x.Height, 1));
        Assert.Empty(cut.FindAll("[data-flare-chart-hit]"));
    }
}
