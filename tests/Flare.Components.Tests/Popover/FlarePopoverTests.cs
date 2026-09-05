using Flare.Infrastructure;
using Flare.Abstractions;
using Flare.Theming;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

public class FlarePopoverTests : FlareTestContext
{
    [Fact]
    public void RendersAnchorElement()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, false));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Popover.Anchor}"));
    }

    [Fact]
    public void PopoverPaperHiddenWhenOpenFalse()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, false));

        Assert.Empty(cut.FindAll($".{Css.Classes.Popover.Paper}"));
    }

    [Fact]
    public void PopoverPaperRenderedWhenOpenTrue()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Popover.Paper}"));
    }

    [Fact]
    public void RendersChildContent_WhenOpen()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .AddChildContent("<span id=\"pop-child\">Pop Content</span>"));

        Assert.NotEmpty(cut.FindAll("#pop-child"));
    }

    [Fact]
    public void RendersAnchorContent()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, false)
            .Add(x => x.AnchorContent, b =>
                b.AddMarkupContent(0, "<button id=\"trigger-btn\">Open</button>")));

        Assert.NotEmpty(cut.FindAll("#trigger-btn"));
    }

    [Fact]
    public void PlacementBottomStart_HasBottomStartClass()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Placement, PopoverPlacement.BottomStart));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Popover.PaperBottomStart}"));
    }

    [Fact]
    public void PlacementTop_HasTopClass()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Placement, PopoverPlacement.Top));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Popover.PaperTop}"));
    }
}

// ------------------------------------------------------------------------------
// FlareTooltip  (5 tests from Wave3)
// ------------------------------------------------------------------------------
