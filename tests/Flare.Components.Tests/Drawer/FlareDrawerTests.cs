using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareDrawerTests : FlareTestContext
{
    [Fact]
    public void RendersTemporary_DefaultVariant()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, false));

        var drawer = cut.Find($".{Css.Classes.Drawer.Root}");
        Assert.DoesNotContain(Css.Classes.Drawer.Permanent, drawer.ClassName);
        Assert.DoesNotContain(Css.Classes.Drawer.Mini, drawer.ClassName);
    }

    [Fact]
    public void RendersPermanent()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Permanent));

        Assert.Contains(Css.Classes.Drawer.Permanent, cut.Find($".{Css.Classes.Drawer.Root}").ClassName);
    }

    [Fact]
    public void RendersMini()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Mini));

        Assert.Contains(Css.Classes.Drawer.Mini, cut.Find($".{Css.Classes.Drawer.Root}").ClassName);
    }

    [Fact]
    public void RendersScrimForTemporaryWhenOpen()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Drawer.Scrim}"));
    }

    [Fact]
    public void NoScrimForPermanent()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Permanent)
            .Add(x => x.Open, true));

        Assert.Empty(cut.FindAll($".{Css.Classes.Drawer.Scrim}"));
    }

    [Fact]
    public void RendersChildContent()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Permanent)
            .AddChildContent("<nav class=\"nav-content\">Nav</nav>"));

        Assert.NotNull(cut.Find(".nav-content"));
    }

    [Fact]
    public void RendersOpenState()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, true));

        Assert.Contains(Css.Classes.Drawer.Open, cut.Find($".{Css.Classes.Drawer.Root}").ClassName);
    }

    [Fact]
    public void RendersClosedState_NoOpenClass()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Temporary)
            .Add(x => x.Open, false));

        Assert.DoesNotContain(Css.Classes.Drawer.Open, cut.Find($".{Css.Classes.Drawer.Root}").ClassName);
    }

    // Every variant must reach the stylesheet. Persistent and Responsive shipped for several releases
    // emitting neither a variant class nor an open class, which left them parked off-screen at
    // translateX(-100%) with no way to come back - the earlier tests only covered the three that worked.
    [Theory]
    [InlineData(DrawerVariant.Permanent, Css.Classes.Drawer.Permanent)]
    [InlineData(DrawerVariant.Mini, Css.Classes.Drawer.Mini)]
    [InlineData(DrawerVariant.Persistent, Css.Classes.Drawer.Persistent)]
    [InlineData(DrawerVariant.Responsive, Css.Classes.Drawer.Responsive)]
    public void EveryNonTemporaryVariantEmitsItsClass(DrawerVariant variant, string expected)
    {
        var cut = Render<FlareDrawer>(p => p.Add(x => x.Variant, variant));

        Assert.Contains(expected, cut.Find($".{Css.Classes.Drawer.Root}").ClassName);
    }

    // The in-flow variants grow from zero width rather than sliding in on a transform, so "open" is a
    // different class for them. Responsive carries both, because which half applies is the breakpoint's
    // decision and the component cannot know it.
    [Theory]
    [InlineData(DrawerVariant.Persistent, false)]
    [InlineData(DrawerVariant.Responsive, true)]
    public void InFlowVariantsExpandRatherThanSlide(DrawerVariant variant, bool alsoSlides)
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, variant)
            .Add(x => x.Open, true));

        var className = cut.Find($".{Css.Classes.Drawer.Root}").ClassName ?? string.Empty;
        Assert.Contains(Css.Classes.Drawer.Expanded, className);
        Assert.Equal(alsoSlides, className.Contains(Css.Classes.Drawer.Open));
    }

    [Fact]
    public void PersistentClosedHasNoExpandedClass()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Persistent)
            .Add(x => x.Open, false));

        Assert.DoesNotContain(Css.Classes.Drawer.Expanded, cut.Find($".{Css.Classes.Drawer.Root}").ClassName);
    }

    // Below the breakpoint Responsive IS the overlay, so it needs the scrim; the marker class is what
    // lets the stylesheet take the scrim away again above the breakpoint.
    [Fact]
    public void ResponsiveOpenRendersAScrimThatCanBeHiddenAboveTheBreakpoint()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Responsive)
            .Add(x => x.Open, true));

        Assert.Contains(Css.Classes.Drawer.ScrimResponsive, cut.Find($".{Css.Classes.Drawer.Scrim}").ClassName);
    }

    [Fact]
    public void PersistentOpenRendersNoScrim()
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, DrawerVariant.Persistent)
            .Add(x => x.Open, true));

        Assert.Empty(cut.FindAll($".{Css.Classes.Drawer.Scrim}"));
    }

    // An in-flow drawer leaves the rest of the page reachable beside it. Announcing it as a modal
    // dialog tells a screen reader the page is inert when it is not.
    [Theory]
    [InlineData(DrawerVariant.Temporary, "dialog", "true")]
    [InlineData(DrawerVariant.Permanent, "navigation", null)]
    [InlineData(DrawerVariant.Persistent, "navigation", null)]
    [InlineData(DrawerVariant.Responsive, "navigation", null)]
    [InlineData(DrawerVariant.Mini, "navigation", null)]
    public void OnlyTheOverlayVariantIsAModalDialog(DrawerVariant variant, string role, string? ariaModal)
    {
        var cut = Render<FlareDrawer>(p => p
            .Add(x => x.Variant, variant)
            .Add(x => x.Open, true));

        var drawer = cut.Find($".{Css.Classes.Drawer.Root}");
        Assert.Equal(role, drawer.GetAttribute("role"));
        Assert.Equal(ariaModal, drawer.GetAttribute("aria-modal"));
    }
}

// ------------------------------------------------------------------------------
// FlareMenu  (8 tests from Wave5)
// ------------------------------------------------------------------------------
