namespace Flare.Components.Tests;

/// <summary>
/// Guards the Weir-dashboard / Deka gap fixes: FlareChart sparkline gets a fixed pixel height (no longer
/// width-driven), and FlareLayoutAppBar gains first-class Height / Dense controls. (The icon-only button
/// glyph-centering fix is CSS-only and verified in the Gallery preview.)
/// </summary>
public sealed class IconLayoutSparklineGapTests : FlareTestContext
{
    private static readonly ChartData _data = new(
        [new ChartSeries("A", [1, 5, 3, 8])],
        ["a", "b", "c", "d"]);

    // -- FlareChart sparkline fixed-pixel height -------------------------------

    [Fact]
    public void Sparkline_PinsFixedPixelHeight()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Sparkline, true)
            .Add(x => x.Height, 120)
            .Add(x => x.Data, _data));

        Assert.Contains("height:120px", cut.Find("svg").GetAttribute("style") ?? "");
    }

    [Fact]
    public void NonSparkline_KeepsAspectRatio_NoFixedHeight()
    {
        var cut = Render<FlareChart>(p => p
            .Add(x => x.Sparkline, false)
            .Add(x => x.Height, 120)
            .Add(x => x.Data, _data));

        Assert.DoesNotContain("height:", cut.Find("svg").GetAttribute("style") ?? "");
    }

    // -- FlareLayoutAppBar Height / Dense --------------------------------------

    [Fact]
    public void AppBar_Height_SetsHeightTokenInline()
    {
        var cut = Render<FlareLayoutAppBar>(p => p.Add(x => x.Height, "50px"));

        Assert.Contains("--flare-layout-appbar-height:50px", cut.Find("header").GetAttribute("style") ?? "");
    }

    [Fact]
    public void AppBar_Dense_AddsModifierClass()
    {
        var cut = Render<FlareLayoutAppBar>(p => p.Add(x => x.Dense, true));

        Assert.Contains("flare-layout-appbar--dense", cut.Find("header").ClassList);
    }

    [Fact]
    public void AppBar_Default_NoHeightOverrideNoDense()
    {
        var cut = Render<FlareLayoutAppBar>();
        var header = cut.Find("header");

        Assert.DoesNotContain("--flare-layout-appbar-height", header.GetAttribute("style") ?? "");
        Assert.DoesNotContain("flare-layout-appbar--dense", header.ClassList);
    }
}
