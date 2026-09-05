using Flare.Abstractions;
using Flare.Infrastructure;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;

namespace Flare.Components.Tests;

/// <summary>
/// Guards the Overlay/Dialog family cross-framework parity work (Tier 1/2/3): tooltip triggers +
/// delay + arrow, menu context-menu / max-height / keep-open, dialog close button + before-close
/// hook, snackbar dedupe / clear, popover hover + width, and dialog drag/resize. Each test pins a
/// capability a competitor library exposes so a regression is caught by the build gate.
/// </summary>
public sealed class OverlayDialogAuditTests : FlareTestContext
{
    public OverlayDialogAuditTests()
    {
        Services.AddSingleton<ISnackbarService, SnackbarService>();
    }

    // -- Tooltip ---------------------------------------------------------------

    [Fact]
    public void Tooltip_DefaultTriggers_HoverAndFocus()
    {
        var cut = Render<FlareTooltip>(p => p.Add(x => x.Content, "hi"));
        var root = cut.Find($".{Css.Classes.Tooltip.Root}");
        Assert.Contains(Css.Classes.Tooltip.TriggerHover, root.ClassList);
        Assert.Contains(Css.Classes.Tooltip.TriggerFocus, root.ClassList);
        Assert.DoesNotContain(Css.Classes.Tooltip.TriggerClick, root.ClassList);
    }

    [Fact]
    public void Tooltip_FocusOnly_DropsHover()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hi")
            .Add(x => x.ShowOnHover, false));
        var root = cut.Find($".{Css.Classes.Tooltip.Root}");
        Assert.DoesNotContain(Css.Classes.Tooltip.TriggerHover, root.ClassList);
        Assert.Contains(Css.Classes.Tooltip.TriggerFocus, root.ClassList);
    }

    [Fact]
    public void Tooltip_ClickTrigger_TogglesOpenClass()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hi")
            .Add(x => x.ShowOnHover, false)
            .Add(x => x.ShowOnClick, true));
        var root = cut.Find($".{Css.Classes.Tooltip.Root}");
        Assert.Contains(Css.Classes.Tooltip.TriggerClick, root.ClassList);
        Assert.DoesNotContain(Css.Classes.Tooltip.Open, cut.Find($".{Css.Classes.Tooltip.Root}").ClassList);

        cut.Find($".{Css.Classes.Tooltip.Root}").Click();
        Assert.Contains(Css.Classes.Tooltip.Open, cut.Find($".{Css.Classes.Tooltip.Root}").ClassList);
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Tooltip.Backdrop}"));
    }

    [Fact]
    public void Tooltip_Disabled_SuppressesTriggers()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hi")
            .Add(x => x.Disabled, true));
        var root = cut.Find($".{Css.Classes.Tooltip.Root}");
        Assert.Contains(Css.Classes.Tooltip.Disabled, root.ClassList);
        Assert.DoesNotContain(Css.Classes.Tooltip.TriggerHover, root.ClassList);
        Assert.DoesNotContain(Css.Classes.Tooltip.TriggerFocus, root.ClassList);
    }

    [Fact]
    public void Tooltip_Arrow_AddsModifier()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hi")
            .Add(x => x.Arrow, true));
        Assert.Contains(Css.Classes.Tooltip.Arrow, cut.Find($".{Css.Classes.Tooltip.Root}").ClassList);
    }

    [Fact]
    public void Tooltip_Delay_EmitsLocalVar()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.Content, "hi")
            .Add(x => x.Delay, 500));
        Assert.Contains("--fc-tt-delay:500ms", cut.Find($".{Css.Classes.Tooltip.Root}").GetAttribute("style"));
    }

    [Fact]
    public void Tooltip_RichContent_AppliesRichClass()
    {
        var cut = Render<FlareTooltip>(p => p
            .Add(x => x.TooltipContent, "<b>rich</b>"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Tooltip.ContentRich}"));
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
        cut.Find($".{Css.Classes.Menu.Activator}").Click();
        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Panel}"));

        // The pointer coordinates go to the placement engine as an anchor rectangle rather than into an
        // inline style, so the marker of "opened at the cursor" is the class, not a pair of pixels: two
        // owners for top/left is exactly what the engine was brought in to end.
        cut.Find($".{Css.Classes.Menu.Activator}").ContextMenu(new MouseEventArgs { ClientX = 120, ClientY = 240 });
        var panel = cut.Find($".{Css.Classes.Menu.Panel}");
        Assert.Contains(Css.Classes.Menu.AtCursor, panel.ClassList);
        Assert.DoesNotContain("left:", panel.GetAttribute("style") ?? "");
    }

    [Fact]
    public void Menu_MaxHeight_AddsScrollClassAndStyle()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(m => m.MaxHeight, "14rem")
            .Add(m => m.Activator, Markup("<span>x</span>")));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();
        var panel = cut.Find($".{Css.Classes.Menu.Panel}");
        Assert.Contains(Css.Classes.Menu.PanelScroll, panel.ClassList);
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

        cut.Find($".{Css.Classes.Menu.Activator}").Click();
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.Panel}"));

        cut.Find($".{Css.Classes.Menu.Item}").Click();
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
    }

    [Fact]
    public void MenuItem_DefaultAutoClose_ClosesMenu()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(m => m.Activator, Markup("<span>x</span>"))
            .Add(m => m.ChildContent, MenuItem(autoClose: true)));

        cut.Find($".{Css.Classes.Menu.Activator}").Click();
        cut.Find($".{Css.Classes.Menu.Item}").Click();
        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
    }

    // -- Snackbar --------------------------------------------------------------

    [Fact]
    public void Snackbar_PreventDuplicate_SuppressesRepeat()
    {
        var cut = Render<FlareSnackbarProvider>();
        var svc = Services.GetRequiredService<ISnackbarService>();
        var opts = new SnackbarOptions { DurationMs = 0, PreventDuplicate = true };

        svc.Show("same", opts);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 1);
        svc.Show("same", opts);

        Assert.Single(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }

    [Fact]
    public void Snackbar_WithoutPreventDuplicate_StacksBoth()
    {
        var cut = Render<FlareSnackbarProvider>();
        var svc = Services.GetRequiredService<ISnackbarService>();

        svc.Show("dup", new SnackbarOptions { DurationMs = 0 });
        svc.Show("dup", new SnackbarOptions { DurationMs = 0 });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 2);

        Assert.Equal(2, cut.FindAll($".{Css.Classes.Snackbar.Root}").Count);
    }

    [Fact]
    public void Snackbar_Clear_DismissesAll()
    {
        var cut = Render<FlareSnackbarProvider>();
        var svc = Services.GetRequiredService<ISnackbarService>();

        svc.Show("a", new SnackbarOptions { DurationMs = 0 });
        svc.Show("b", new SnackbarOptions { DurationMs = 0 });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 2);

        svc.Clear();
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 0);
        Assert.Empty(cut.FindAll($".{Css.Classes.Snackbar.Root}"));
    }

    [Fact]
    public void Snackbar_Remove_DismissesById()
    {
        var cut = Render<FlareSnackbarProvider>();
        var svc = Services.GetRequiredService<ISnackbarService>();
        var id = Guid.NewGuid();

        svc.Show(new SnackbarMessage(id, "one", SnackbarSeverity.Normal, 0));
        svc.Show("two", new SnackbarOptions { DurationMs = 0 });
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 2);

        svc.Remove(id);
        cut.WaitForState(() => cut.FindAll($".{Css.Classes.Snackbar.Root}").Count == 1);
        Assert.Contains("two", cut.Markup);
        Assert.DoesNotContain("one", cut.Markup);
    }

    [Fact]
    public void Snackbar_CustomContent_IsRendered()
    {
        var cut = Render<FlareSnackbarProvider>();
        var svc = Services.GetRequiredService<ISnackbarService>();
        RenderFragment body = b => b.AddMarkupContent(0, "<i id=\"custom-snack\">rich</i>");

        svc.Show(body, new SnackbarOptions { DurationMs = 0 });
        cut.WaitForState(() => cut.FindAll("#custom-snack").Count == 1);

        Assert.NotEmpty(cut.FindAll("#custom-snack"));
    }

    // -- Popover ---------------------------------------------------------------

    [Fact]
    public void Popover_MatchAnchorWidth_AddsClass()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.MatchAnchorWidth, true)
            .Add(x => x.AnchorContent, Markup("<span>a</span>"))
            .Add(x => x.ChildContent, Markup("<span>c</span>")));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Popover.PaperMatchWidth}"));
    }

    [Fact]
    public void Popover_MaxHeight_AppliesStyle()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.MaxHeight, "12rem")
            .Add(x => x.AnchorContent, Markup("<span>a</span>"))
            .Add(x => x.ChildContent, Markup("<span>c</span>")));

        Assert.Contains("max-height:12rem", cut.Find($".{Css.Classes.Popover.Paper}").GetAttribute("style") ?? "");
    }

    [Fact]
    public void Popover_HoverTrigger_RaisesOpenOnEnter()
    {
        var opened = false;
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Trigger, PopoverTrigger.Hover)
            .Add(x => x.Delay, 0)
            .Add(x => x.AnchorContent, Markup("<span>a</span>"))
            .Add(x => x.OpenChanged, (bool v) => opened = v));

        cut.Find($".{Css.Classes.Popover.Anchor}").MouseEnter();
        cut.WaitForState(() => opened);
        Assert.True(opened);
    }

    [Fact]
    public void Popover_HoverTrigger_NoScrim()
    {
        var cut = Render<FlarePopover>(p => p
            .Add(x => x.Open, true)
            .Add(x => x.Trigger, PopoverTrigger.Hover)
            .Add(x => x.AnchorContent, Markup("<span>a</span>"))
            .Add(x => x.ChildContent, Markup("<span>c</span>")));

        // A hover popover is not modal, so it must not render the dismiss backdrop.
        Assert.Empty(cut.FindAll($".{Css.Classes.Popover.Backdrop}"));
    }

    // -- Dialog ----------------------------------------------------------------

    [Fact]
    public void Dialog_CloseButton_RendersAndCloses()
    {
        bool? last = null;
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.ShowCloseButton, true)
            .Add(x => x.VisibleChanged, (bool v) => last = v));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Dialog.CloseButton}"));
        cut.Find($".{Css.Classes.Dialog.CloseButton}").Click();
        Assert.False(last);
    }

    [Fact]
    public void Dialog_BeforeClose_CanVetoClose()
    {
        bool? last = null;
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.ShowCloseButton, true)
            .Add(x => x.BeforeClose, _ => ValueTask.FromResult(false))
            .Add(x => x.VisibleChanged, (bool v) => last = v));

        cut.Find($".{Css.Classes.Dialog.CloseButton}").Click();
        Assert.Null(last); // vetoed - VisibleChanged never fired
    }

    [Fact]
    public void Dialog_Draggable_AddsHandleAndClass()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.Title, "Move me")
            .Add(x => x.Draggable, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Dialog.Draggable}"));
        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Dialog.DragHandle}"));
    }

    [Fact]
    public void Dialog_Resizable_AddsGripper()
    {
        var cut = Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.Resizable, true));

        Assert.NotEmpty(cut.FindAll($".{Css.Classes.Dialog.Resizer}"));
    }

    [Fact]
    public void Dialog_CloseOnNavigation_ClosesOnNavigate()
    {
        bool? last = null;
        Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.CloseOnNavigation, true)
            .Add(x => x.VisibleChanged, (bool v) => last = v));

        Services.GetRequiredService<NavigationManager>().NavigateTo("other");
        Assert.False(last);
    }

    [Fact]
    public void Dialog_CloseOnNavigationFalse_StaysOpen()
    {
        bool? last = null;
        Render<FlareDialog>(p => p
            .Add(x => x.Visible, true)
            .Add(x => x.CloseOnNavigation, false)
            .Add(x => x.VisibleChanged, (bool v) => last = v));

        Services.GetRequiredService<NavigationManager>().NavigateTo("other");
        Assert.Null(last);
    }
}
