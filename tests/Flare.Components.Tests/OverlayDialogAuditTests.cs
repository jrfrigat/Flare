using Flare.Abstractions;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// Guards the Overlay/Dialog family cross-framework parity work (Tier 1/2/3): tooltip triggers +
/// delay + arrow, menu context-menu / max-height / keep-open, dialog close button + before-close
/// hook, snackbar dedupe / clear, popover hover + width, and dialog drag/resize. Each test pins a
/// capability a competitor library exposes so a regression is caught by the build gate.
/// </summary>
public sealed class OverlayDialogAuditTests : FlareTestContext
{
    // -- Tooltip ---------------------------------------------------------------

    [Fact]
    public void Tooltip_DefaultTriggers_HoverAndFocus()
    {
        var cut = Render<FlareTooltip>(p => p.Add(x => x.Content, "hi"));
        var root = cut.Find(".flare-tooltip");
        Assert.Contains("flare-tooltip--hover", root.ClassList);
        Assert.Contains("flare-tooltip--focus", root.ClassList);
        Assert.DoesNotContain("flare-tooltip--click", root.ClassList);
    }

    [Fact]
    public void Tooltip_FocusOnly_DropsHover()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hi")
            .Add(x => x.ShowOnHover, false));
        var root = cut.Find(".flare-tooltip");
        Assert.DoesNotContain("flare-tooltip--hover", root.ClassList);
        Assert.Contains("flare-tooltip--focus", root.ClassList);
    }

    [Fact]
    public void Tooltip_ClickTrigger_TogglesOpenClass()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hi")
            .Add(x => x.ShowOnHover, false)
            .Add(x => x.ShowOnClick, true));
        var root = cut.Find(".flare-tooltip");
        Assert.Contains("flare-tooltip--click", root.ClassList);
        Assert.DoesNotContain("flare-tooltip--open", cut.Find(".flare-tooltip").ClassList);

        cut.Find(".flare-tooltip").Click();
        Assert.Contains("flare-tooltip--open", cut.Find(".flare-tooltip").ClassList);
        Assert.NotEmpty(cut.FindAll(".flare-tooltip__backdrop"));
    }

    [Fact]
    public void Tooltip_Disabled_SuppressesTriggers()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hi")
            .Add(x => x.Disabled, true));
        var root = cut.Find(".flare-tooltip");
        Assert.Contains("flare-tooltip--disabled", root.ClassList);
        Assert.DoesNotContain("flare-tooltip--hover", root.ClassList);
        Assert.DoesNotContain("flare-tooltip--focus", root.ClassList);
    }

    [Fact]
    public void Tooltip_Arrow_AddsModifier()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hi")
            .Add(x => x.Arrow, true));
        Assert.Contains("flare-tooltip--arrow", cut.Find(".flare-tooltip").ClassList);
    }

    [Fact]
    public void Tooltip_Delay_EmitsLocalVar()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hi")
            .Add(x => x.Delay, 500));
        Assert.Contains("--fc-tt-delay:500ms", cut.Find(".flare-tooltip").GetAttribute("style"));
    }

    [Fact]
    public void Tooltip_RichContent_AppliesRichClass()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.TooltipContent, "<b>rich</b>"));
        Assert.NotEmpty(cut.FindAll(".flare-tooltip__content--rich"));
    }

    // -- Menu ------------------------------------------------------------------

    private static RenderFragment Markup(string html) => b => b.AddMarkupContent(0, html);

    [Fact]
    public void Menu_RightClickActivation_OpensAtCursor_LeftClickIgnored()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(m => m.Activation, MenuActivation.RightClick)
            .Add(m => m.PositionAtCursor, true)
            .Add(m => m.Activator, Markup("<span>x</span>")));

        // Left click must not open a right-click menu.
        cut.Find(".flare-menu__activator").Click();
        Assert.Empty(cut.FindAll(".flare-menu__panel"));

        cut.Find(".flare-menu__activator").ContextMenu(new MouseEventArgs { ClientX = 120, ClientY = 240 });
        var panel = cut.Find(".flare-menu__panel");
        Assert.Contains("flare-menu__panel--at-cursor", panel.ClassList);
        var style = panel.GetAttribute("style") ?? "";
        Assert.Contains("left:120px", style);
        Assert.Contains("top:240px", style);
    }

    [Fact]
    public void Menu_MaxHeight_AddsScrollClassAndStyle()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(m => m.MaxHeight, "14rem")
            .Add(m => m.Activator, Markup("<span>x</span>")));

        cut.Find(".flare-menu__activator").Click();
        var panel = cut.Find(".flare-menu__panel");
        Assert.Contains("flare-menu__panel--scroll", panel.ClassList);
        Assert.Contains("max-height:14rem", panel.GetAttribute("style") ?? "");
    }

    private static RenderFragment MenuItem(bool autoClose) => b =>
    {
        b.OpenComponent<FlareMenuItem>(0);
        b.AddAttribute(1, nameof(FlareMenuItem.AutoClose), autoClose);
        b.AddAttribute(2, nameof(FlareMenuItem.ChildContent), (RenderFragment)(c => c.AddContent(0, "Toggle")));
        b.CloseComponent();
    };

    [Fact]
    public void MenuItem_AutoCloseFalse_KeepsMenuOpen()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(m => m.Activator, Markup("<span>x</span>"))
            .Add(m => m.ChildContent, MenuItem(autoClose: false)));

        cut.Find(".flare-menu__activator").Click();
        Assert.NotEmpty(cut.FindAll(".flare-menu__panel"));

        cut.Find(".flare-menu-item").Click();
        Assert.NotEmpty(cut.FindAll(".flare-menu__panel"));
    }

    [Fact]
    public void MenuItem_DefaultAutoClose_ClosesMenu()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(m => m.Activator, Markup("<span>x</span>"))
            .Add(m => m.ChildContent, MenuItem(autoClose: true)));

        cut.Find(".flare-menu__activator").Click();
        cut.Find(".flare-menu-item").Click();
        Assert.Empty(cut.FindAll(".flare-menu__panel"));
    }
}
