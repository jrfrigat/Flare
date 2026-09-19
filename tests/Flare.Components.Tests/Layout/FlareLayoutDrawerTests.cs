using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareLayoutDrawer owns its own open state and reports a grid track to the layout per variant:
// Mini collapses to an icon rail, Persistent reserves a zero-width track when closed, Temporary floats.
public class FlareLayoutDrawerTests : FlareTestContext
{
    private IRenderedComponent<FlareLayoutDrawer> RenderDrawer(DrawerVariant variant, bool open)
        => Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Variant, variant)
            .Add(d => d.Open, open)
            .Add(d => d.Width, "16rem")
            .Add(d => d.RailWidth, "5rem"));

    [Fact]
    public void MiniCollapsed_IsIconRail_ReservesRailTrack()
    {
        var d = RenderDrawer(DrawerVariant.Mini, open: false).Instance;
        Assert.True(d.IsCollapsedRail);
        Assert.False(d.IsOpen);
        Assert.True(d.ReservesTrack(false));
        Assert.Equal("5rem", d.TrackWidth(false));
    }

    [Fact]
    public void MiniOpen_IsFullWidth_NotCollapsed()
    {
        var d = RenderDrawer(DrawerVariant.Mini, open: true).Instance;
        Assert.False(d.IsCollapsedRail);
        Assert.Equal("16rem", d.TrackWidth(false));
    }

    [Fact]
    public void PersistentClosed_ReservesZeroWidthTrack()
    {
        var d = RenderDrawer(DrawerVariant.Persistent, open: false).Instance;
        Assert.True(d.ReservesTrack(false));   // keeps its track so the width can animate
        Assert.Equal("0", d.TrackWidth(false));
    }

    [Fact]
    public void PersistentOpen_ReservesFullTrack()
        => Assert.Equal("16rem", RenderDrawer(DrawerVariant.Persistent, open: true).Instance.TrackWidth(false));

    [Fact]
    public void Temporary_Floats_NeverReservesTrack()
    {
        var d = RenderDrawer(DrawerVariant.Temporary, open: true).Instance;
        Assert.False(d.ReservesTrack(false));
        Assert.True(d.IsOverlayOpen(false));
    }

    [Fact]
    public void TemporaryOpen_IsModalDialog_WithAriaLabel()
    {
        var cut = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Variant, DrawerVariant.Temporary)
            .Add(d => d.Open, true)
            .Add(d => d.AriaLabel, "Navigation"));

        var nav = cut.Find("nav");
        Assert.Equal("dialog", nav.GetAttribute("role"));
        Assert.Equal("true", nav.GetAttribute("aria-modal"));
        Assert.Equal("Navigation", nav.GetAttribute("aria-label"));
    }

    [Fact]
    public void PersistentOpen_IsNotModal_AndNotInert()
    {
        var cut = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Variant, DrawerVariant.Persistent)
            .Add(d => d.Open, true)
            .Add(d => d.Width, "16rem"));

        var nav = cut.Find("nav");
        Assert.False(nav.HasAttribute("role"));
        Assert.False(nav.HasAttribute("aria-modal"));
        Assert.False(nav.HasAttribute("inert"));
    }

    [Fact]
    public void ClosedPushDrawer_IsInert_NotKeyboardReachable()
    {
        var cut = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Variant, DrawerVariant.Persistent)
            .Add(d => d.Open, false)
            .Add(d => d.Width, "16rem"));

        // A closed push drawer collapses to a 0-width track but keeps its links in the DOM; inert
        // removes them from the tab order + a11y tree instead of leaving focusable links under aria-hidden.
        Assert.True(cut.Find("nav").HasAttribute("inert"));
    }

    [Fact]
    public void Mobile_PushDrawerGoesOffCanvas()
    {
        var d = RenderDrawer(DrawerVariant.Mini, open: false).Instance;
        Assert.False(d.ReservesTrack(true));   // off-canvas on mobile
        Assert.Equal("0", d.TrackWidth(true));
    }

    [Fact]
    public async Task Toggle_FlipsOpen_AndRaisesChanged()
    {
        var changed = false;
        var cut = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Variant, DrawerVariant.Persistent)
            .Add(d => d.Open, false)
            .Add(d => d.OpenChanged, EventCallback.Factory.Create<bool>(this, v => changed = v)));
        await cut.InvokeAsync(() => cut.Instance.ToggleAsync());
        Assert.True(cut.Instance.IsOpen);
        Assert.True(changed);
    }
}

// FlareLayoutDrawer.ContentPadding: the shell drawer and the standalone FlareDrawer wrap the same panel
// surface, so they answer the same step with the same inset. They do it by sharing the six modifier
// classes rather than by each declaring a scale, which is what makes "the same value" true by
// construction instead of by two lists agreeing.
public class FlareLayoutDrawerContentPaddingTests : FlareTestContext
{
    [Fact]
    public void DefaultsToFullBleed()
    {
        var cut = Render<FlareLayoutDrawer>(p => p.Add(d => d.Open, true));

        var root = cut.Find("nav");
        Assert.DoesNotContain("content-pad", root.ClassName);
        Assert.DoesNotContain("--_content-pad-x", root.GetAttribute("style") ?? "");
    }

    [Theory]
    [InlineData(FlareSpacing.XXSmall, Css.Classes.Drawer.ContentPadXxSmall)]
    [InlineData(FlareSpacing.XSmall, Css.Classes.Drawer.ContentPadXSmall)]
    [InlineData(FlareSpacing.Small, Css.Classes.Drawer.ContentPadSmall)]
    [InlineData(FlareSpacing.Medium, Css.Classes.Drawer.ContentPadMedium)]
    [InlineData(FlareSpacing.Large, Css.Classes.Drawer.ContentPadLarge)]
    [InlineData(FlareSpacing.XLarge, Css.Classes.Drawer.ContentPadXLarge)]
    public void Step_AppliesTheMatchingClass(FlareSpacing step, string expected)
    {
        var cut = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Open, true)
            .Add(d => d.ContentPadding, step));

        Assert.Contains(expected, cut.Find("nav").ClassName);
    }

    // The two wrappers must select the SAME class for a step. Compared rather than asserted against a
    // literal, so the day one of them grows a scale of its own this fails instead of drifting quietly.
    [Theory]
    [InlineData(FlareSpacing.XXSmall)]
    [InlineData(FlareSpacing.Small)]
    [InlineData(FlareSpacing.Medium)]
    [InlineData(FlareSpacing.XLarge)]
    public void Step_MatchesTheStandaloneDrawer(FlareSpacing step)
    {
        var shell = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Open, true)
            .Add(d => d.ContentPadding, step));
        var standalone = Render<FlareDrawer>(p => p
            .Add(d => d.Open, true)
            .Add(d => d.ContentPadding, step)
            .Add(d => d.ChildContent, "<p>body</p>"));

        var padClass = shell.Find("nav").ClassName.Split(' ')
            .Single(c => c.Contains("content-pad", StringComparison.Ordinal));

        Assert.Contains(padClass, standalone.Find($".{Css.Classes.Drawer.Root}").ClassName);
    }

    [Fact]
    public void Step_CarriesNoGeometryInline()
    {
        var cut = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Open, true)
            .Add(d => d.ContentPadding, FlareSpacing.Large));

        Assert.DoesNotContain("--_content-pad-x", cut.Find("nav").GetAttribute("style") ?? "");
    }

    [Fact]
    public void Custom_EmitsTheRawValue()
    {
        var cut = Render<FlareLayoutDrawer>(p => p
            .Add(d => d.Open, true)
            .Add(d => d.ContentPadding, FlareSpacing.Custom)
            .Add(d => d.ContentPaddingValue, "3.5rem"));

        Assert.Contains("--_content-pad-x:3.5rem", cut.Find("nav").GetAttribute("style") ?? "");
    }
}
