using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlareOverlayTests : FlareTestContext
{
    [Fact]
    public void HiddenWhenOpenFalse()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.Overlay.Root}"));
    }

    [Fact]
    public void RendersWhenOpenTrue()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Overlay.Root}"));
    }

    [Fact]
    public void RendersChildContent_WhenOpen()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true)
            .AddChildContent("<span id=\"overlay-child\">Content</span>"));

        Assert.NotEmpty(cut.FindAll("#overlay-child"));
    }

    [Fact]
    public void ZIndex_AppliedInStyle()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.ZIndex, 9999));

        var style = cut.Find($".{Css.Classes.Overlay.Root}").GetAttribute("style") ?? "";
        Assert.Contains("9999", style);
    }

    [Fact]
    public void Opacity_AppliedInStyle()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Opacity, 0.75));

        var style = cut.Find($".{Css.Classes.Overlay.Root}").GetAttribute("style") ?? "";
        Assert.Contains("0.75", style);
    }

    [Fact]
    public void Absolute_HasAbsoluteClass()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Absolute, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Overlay.Absolute}"));
    }

    [Fact]
    public void NotAbsolute_NoAbsoluteClass()
    {
        var cut = Render<FlareOverlay>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Absolute, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.Overlay.Absolute}"));
    }
}

// ------------------------------------------------------------------------------
// FlarePopover  (7 tests from Wave5)
// ------------------------------------------------------------------------------
