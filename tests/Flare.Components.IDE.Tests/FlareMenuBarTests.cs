using AngleSharp.Dom;
using Flare.Components;
using Flare.Components.IDE;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A menu bar coordinates its top-level menus: one open at a time, click or hover switches, the bar
/// owns a single backdrop, Escape closes and focus returns to that menu's own activator.
/// </summary>
public class FlareMenuBarTests : FlareTestContext
{
    private static RenderFragment<FlareMenuActivatorContext> Activator(string title) => ctx => b =>
    {
        b.OpenElement(0, "button");
        b.AddMultipleAttributes(1, ctx.Attributes);
        b.AddContent(2, title);
        b.CloseElement();
    };

    private static RenderFragment Menu(string title, string item, string ariaLabel) =>
        Menu(title, ariaLabel, b =>
        {
            b.OpenComponent<FlareMenuItem>(0);
            b.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(x => x.AddContent(0, item)));
            b.CloseComponent();
        });

    private static RenderFragment Menu(string title, string ariaLabel, RenderFragment content, bool freeContent = false) => b =>
    {
        b.OpenComponent<FlareMenu>(0);
        b.AddAttribute(1, nameof(FlareMenu.AriaLabel), ariaLabel);
        b.AddAttribute(2, nameof(FlareMenu.ChildContent), content);
        b.AddAttribute(3, nameof(FlareMenu.Activator), Activator(title));
        b.AddAttribute(4, nameof(FlareMenu.FreeContent), freeContent);
        b.CloseComponent();
    };

    private IRenderedComponent<FlareMenuBar> RenderBar() =>
        Render<FlareMenuBar>(p => p.Add(x => x.ChildContent, (RenderFragment)(b =>
        {
            b.AddContent(0, Menu("File", "New", "File menu"));
            b.AddContent(1, Menu("Edit", "Copy", "Edit menu"));
        })));

    // Clicks land on the wrapper that owns the handler, the same element the menu tests drive.
    private static IElement[] ActivatorWrappers(IRenderedComponent<FlareMenuBar> cut) =>
        cut.FindAll($".{Css.Classes.Menu.Activator}").ToArray();

    private static string? OpenPanelLabel(IRenderedComponent<FlareMenuBar> cut) =>
        cut.FindAll($".{Css.Classes.Menu.Panel}").SingleOrDefault()?.GetAttribute("aria-label");

    [Fact]
    public async Task OpeningAMenu_RendersOneSharedBackdropForTheBar()
    {
        var cut = RenderBar();
        await cut.InvokeAsync(() => ActivatorWrappers(cut)[0].Click());

        Assert.Equal("File menu", OpenPanelLabel(cut));
        Assert.Single(cut.FindAll($".{Css.Classes.Menu.Backdrop}"));
        Assert.Single(cut.FindAll($".{Css.Classes.Ide.MenuBar.Open}"));
    }

    [Fact]
    public async Task ClickingAnotherTopLevelMenu_SwitchesInOneClick()
    {
        var cut = RenderBar();
        await cut.InvokeAsync(() => ActivatorWrappers(cut)[0].Click());
        await cut.InvokeAsync(() => ActivatorWrappers(cut)[1].Click());

        // The backdrop did not swallow the second click: Edit is open and File is gone.
        Assert.Equal("Edit menu", OpenPanelLabel(cut));
        Assert.Single(cut.FindAll($".{Css.Classes.Menu.Backdrop}"));
    }

    [Fact]
    public async Task HoveringAnotherTopLevelMenu_SwitchesWithoutAClick()
    {
        var cut = RenderBar();
        await cut.InvokeAsync(() => ActivatorWrappers(cut)[0].Click());
        await cut.InvokeAsync(() => ActivatorWrappers(cut)[1].TriggerEvent("onmouseenter", new MouseEventArgs()));

        Assert.Equal("Edit menu", OpenPanelLabel(cut));
        Assert.Single(cut.FindAll($".{Css.Classes.Menu.Panel}"));
    }

    [Fact]
    public async Task NestedMenu_OpensIndependentlyOfTheBar()
    {
        var nested = Menu("Export", "PDF", "Export menu");
        var cut = Render<FlareMenuBar>(p => p.Add(x => x.ChildContent,
            Menu("File", "File panel", nested, freeContent: true)));
        await cut.InvokeAsync(() => ActivatorWrappers(cut)[0].Click());

        var activator = $".{Css.Classes.Menu.Panel} .{Css.Classes.Menu.Activator}";
        // A menu inside the panel is outside the bar's coordination, so it does not subscribe to
        // hover at all and cannot switch the bar by a pointer passing over it.
        Assert.False(cut.Find(activator).HasAttribute("blazor:onmouseenter"));
        Assert.Equal("File panel", OpenPanelLabel(cut));

        await cut.InvokeAsync(() => cut.Find(activator).Click());
        var panels = cut.FindAll($".{Css.Classes.Menu.Panel}");
        Assert.Equal(new[] { "File panel", "Export menu" }, panels.Select(p => p.GetAttribute("aria-label")));
        Assert.Equal(2, cut.FindAll($".{Css.Classes.Menu.Backdrop}").Count);

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Panel} .{Css.Classes.Menu.Backdrop}").Click());
        Assert.Equal("File panel", OpenPanelLabel(cut));
        Assert.Single(cut.FindAll($".{Css.Classes.Menu.Backdrop}"));
    }

    [Fact]
    public async Task ClickingTheHoverOpenedMenu_KeepsItOpen()
    {
        var cut = RenderBar();
        await cut.InvokeAsync(() => ActivatorWrappers(cut)[0].Click());
        await cut.InvokeAsync(() => ActivatorWrappers(cut)[1].TriggerEvent("onmouseenter", new MouseEventArgs()));
        await cut.InvokeAsync(() => ActivatorWrappers(cut)[1].Click());

        // The click confirms the hover switch; it must not toggle the menu shut.
        Assert.Equal("Edit menu", OpenPanelLabel(cut));
    }

    [Fact]
    public async Task ClickingTheSharedBackdrop_ClosesTheOpenMenu()
    {
        var cut = RenderBar();
        await cut.InvokeAsync(() => ActivatorWrappers(cut)[0].Click());
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Backdrop}").Click());

        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Backdrop}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Ide.MenuBar.Open}"));
    }

    [Fact]
    public async Task Escape_ClosesTheOpenMenuAndRestoresFocusToItsOwnActivator()
    {
        var module = JSInterop.SetupModule("./_content/Flare.Components/js/flare-overlay.js");
        var cut = RenderBar();
        await cut.InvokeAsync(() => ActivatorWrappers(cut)[0].Click());

        var fileMenu = cut.FindComponents<FlareMenu>().Single(m => m.Instance.AriaLabel == "File menu");
        await cut.InvokeAsync(() => fileMenu.Instance.CloseFromEscape());

        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Panel}"));
        Assert.Empty(cut.FindAll($".{Css.Classes.Menu.Backdrop}"));
        cut.WaitForAssertion(() => module.VerifyInvoke("focusFirstInDialog"));
    }
}
