using Flare.Components.IDE;

namespace Flare.Components.Tests;

/// <summary>
/// FlareRibbon as a WAI-ARIA tab list (Draftline STORY-17): Left/Right (mirrored in right-to-left text) and
/// Home/End move between the enabled tabs, and a disabled tab is announced as unavailable and skipped.
/// </summary>
public class RibbonKeyboardTests : FlareTestContext
{
    public RibbonKeyboardTests() => Services.AddFlareIde();

    private IRenderedComponent<FlareRibbon> RenderRibbon(bool firstDisabled = false) => Render<FlareRibbon>(p => p
        .AddChildContent<FlareRibbonTab>(t => t.Add(x => x.Title, "Home").Add(x => x.Disabled, firstDisabled))
        .AddChildContent<FlareRibbonTab>(t => t.Add(x => x.Title, "Insert").Add(x => x.Disabled, true))
        .AddChildContent<FlareRibbonTab>(t => t.Add(x => x.Title, "View")));

    private static string Active(IRenderedComponent<FlareRibbon> cut) =>
        cut.Find("[role=tab][aria-selected=true]").TextContent.Trim();

    private static Task Press(IRenderedComponent<FlareRibbon> cut, string key, bool rtl = false) =>
        cut.InvokeAsync(() => cut.Instance.OnTabNavigation(key, rtl));

    [Fact]
    public void DisabledTab_IsAnnouncedAsUnavailable()
    {
        var tabs = RenderRibbon().FindAll("[role=tab]");

        Assert.Equal([null, "true", null], tabs.Select(t => t.GetAttribute("aria-disabled")));
    }

    [Fact]
    public void FirstEnabledTab_StartsActive() => Assert.Equal("View", Active(RenderRibbon(firstDisabled: true)));

    [Fact]
    public async Task ArrowsSkipTheDisabledTabAndWrap()
    {
        var cut = RenderRibbon();

        await Press(cut, "ArrowRight");
        Assert.Equal("View", Active(cut));
        await Press(cut, "ArrowRight");
        Assert.Equal("Home", Active(cut));
        await Press(cut, "ArrowLeft");
        Assert.Equal("View", Active(cut));
    }

    [Fact]
    public async Task HomeAndEnd_GoToTheFirstAndLastEnabledTab()
    {
        var cut = RenderRibbon();

        await Press(cut, "End");
        Assert.Equal("View", Active(cut));
        await Press(cut, "Home");
        Assert.Equal("Home", Active(cut));
    }

    [Fact]
    public async Task RightToLeft_MirrorsTheArrows()
    {
        var cut = RenderRibbon();

        await Press(cut, "ArrowLeft", rtl: true);
        Assert.Equal("View", Active(cut));
    }
}
