using Flare.Components.Services;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

/// <summary>
/// Where a floating panel sits relative to its anchor is the theme's to decide: a popover and a tooltip
/// hand the placement engine the name of the theme token for the distance, and fall back to a number only
/// when the caller set one on the instance.
/// </summary>
public sealed class FloatingPanelGapTests : FlareTestContext
{
    private const string Module = "./_content/Flare.Components/js/flare-overlay.js";

    private static RenderFragment Markup(string html) => b => b.AddMarkupContent(0, html);

    private AnchoredPanelOptions LastPlacement(BunitJSModuleInterop module) =>
        (AnchoredPanelOptions)module.Invocations["positionAnchoredPanel"][^1].Arguments[3]!;

    [Fact]
    public void Popover_WithoutOffset_AsksTheEngineForTheThemeToken()
    {
        var module = JSInterop.SetupModule(Module);
        Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.AnchorContent, Markup("<span>x</span>"))
            .Add(x => x.ChildContent, Markup("<span>body</span>")));

        Assert.Equal(Css.Tokens.PopoverPopup.Offset, LastPlacement(module).GapToken);
    }

    [Fact]
    public void Popover_ExplicitOffset_WinsOverTheToken()
    {
        var module = JSInterop.SetupModule(Module);
        Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Offset, 20)
            .Add(x => x.AnchorContent, Markup("<span>x</span>"))
            .Add(x => x.ChildContent, Markup("<span>body</span>")));

        var options = LastPlacement(module);
        Assert.Null(options.GapToken);
        Assert.Equal(20, options.Gap);
    }

    // The menu used a hard 4px for every theme; flush Material menus and gapped Fluent ones need the theme.
    [Fact]
    public void Menu_AsksTheEngineForTheThemeToken()
    {
        var module = JSInterop.SetupModule(Module);
        var cut = Render<FlareMenu>();

        cut.Find($".{Css.Classes.Menu.Activator}").Click();

        Assert.Equal(Css.Tokens.MenuPanel.Offset, LastPlacement(module).GapToken);
    }

    // A tooltip revealed from C# (Open, or a click) used to hand the engine its 4px default while the
    // hovered bubble read the theme token - the same tooltip sat at two distances.
    [Fact]
    public void Tooltip_OpenedFromCode_UsesTheSameTokenAsTheHoverPath()
    {
        var module = JSInterop.SetupModule(Module);
        Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hello")
            .Add(x => x.Open, true)
            .Add(x => x.Placement, Placement.BottomEnd)
            .Add(x => x.ChildContent, Markup("<span>x</span>")));

        var options = LastPlacement(module);
        Assert.Equal(Css.Tokens.TooltipPopup.Offset, options.GapToken);
        Assert.Equal(PanelPlacement.BottomEnd, options.Placement);
    }

    // The page-wide hover listener has no C# to ask, so the full placement rides on the root.
    [Theory]
    [InlineData(Placement.TopStart, "top-start", "top")]
    [InlineData(Placement.BottomEnd, "bottom-end", "bottom")]
    [InlineData(Placement.Right, "right", "right")]
    public void Tooltip_CarriesItsFullPlacementForTheHoverListener(Placement placement, string full, string side)
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hello")
            .Add(x => x.Placement, placement)
            .Add(x => x.ChildContent, Markup("<span>x</span>")));

        Assert.Equal(full, cut.Find($".{Css.Classes.Tooltip.Root}").GetAttribute("data-flare-placement"));
        Assert.Equal(side, cut.Find($".{Css.Classes.Tooltip.Content}").GetAttribute("data-flare-side"));
    }
}
