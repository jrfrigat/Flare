using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareSubMenuCloseChainTests : FlareTestContext
{
    private static string RootPanel => $".{Css.Classes.Menu.Panel}";
    private static string SubmenuPanel => $".{Css.Classes.Menu.SubmenuPanel}";
    private static string SubmenuTrigger => $"[role=menu] [aria-haspopup=menu]";

    private int _toggles;
    private int _actions;

    private static RenderFragment<FlareMenuActivatorContext> ActivatorButton() => ctx => b =>
    {
        b.OpenElement(0, "button");
        b.AddMultipleAttributes(1, ctx.Attributes);
        b.AddContent(2, "open");
        b.CloseElement();
    };

    private RenderFragment Leaf(string label, bool autoClose = true, string? href = null) => b =>
    {
        b.OpenComponent<FlareMenuItem>(0);
        b.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(c => c.AddContent(0, label)));
        b.AddAttribute(2, nameof(FlareMenuItem.OnClick), EventCallback.Factory.Create(this, () => _actions++));
        b.AddAttribute(3, nameof(FlareMenuItem.AutoClose), autoClose);
        if (href is not null) b.AddAttribute(4, nameof(FlareMenuItem.Href), href);
        b.CloseComponent();
    };

    private RenderFragment Submenu(string label, RenderFragment children) => b =>
    {
        b.OpenComponent<FlareSubMenu>(0);
        b.AddAttribute(1, nameof(FlareSubMenu.Label), label);
        b.AddAttribute(2, nameof(FlareSubMenu.HideDelay), 0);
        b.AddAttribute(3, nameof(FlareSubMenu.ChildContent), children);
        b.CloseComponent();
    };

    private IRenderedComponent<FlareMenu> RenderMenu(RenderFragment content) =>
        Render<FlareMenu>(p => p
            .Add(m => m.Activator, ActivatorButton())
            .Add(m => m.OnToggle, EventCallback.Factory.Create<bool>(this, open => { if (!open) _toggles++; }))
            .Add(m => m.ChildContent, content));

    private static async Task OpenMenuAsync(IRenderedComponent<FlareMenu> cut)
    {
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Activator} button").Click(new MouseEventArgs { Detail = 1 }));
        await cut.InvokeAsync(() => cut.Find(SubmenuTrigger).Click());
    }

    private static async Task OpenNestedAsync(IRenderedComponent<FlareMenu> cut)
    {
        await OpenMenuAsync(cut);
        await cut.InvokeAsync(() => cut.Find($"{SubmenuPanel} [aria-haspopup=menu]").Click());
    }

    private static IElement Child(IRenderedComponent<FlareMenu> cut, int submenuIndex) =>
        cut.FindAll(SubmenuPanel)[submenuIndex].QuerySelectorAll("[role=menuitem]")[0];

    [Fact]
    public async Task PointerClickOnASubmenuItem_ClosesTheWholeChainOnce()
    {
        var cut = RenderMenu(Submenu("More", Leaf("First")));
        await OpenMenuAsync(cut);
        Assert.Single(cut.FindAll(RootPanel));
        Assert.Single(cut.FindAll(SubmenuPanel));

        await cut.InvokeAsync(() => Child(cut, 0).Click());

        Assert.Empty(cut.FindAll(RootPanel));
        Assert.Equal(1, _actions);
        Assert.Equal(1, _toggles);
    }

    [Fact]
    public async Task PointerClickOnALinkAtDepthTwo_ClosesTheWholeChainOnce()
    {
        var cut = RenderMenu(Submenu("More", Submenu("Inner", Leaf("Deep", href: "/target"))));
        await OpenNestedAsync(cut);
        Assert.Equal(2, cut.FindAll(SubmenuPanel).Count);

        await cut.InvokeAsync(() => Child(cut, 1).Click());

        Assert.Empty(cut.FindAll(RootPanel));
        Assert.Equal(1, _actions);
        Assert.Equal(1, _toggles);
    }

    [Fact]
    public async Task AutoCloseFalse_KeepsTheSubmenuAndTheMenuOpen()
    {
        var cut = RenderMenu(Submenu("More", Leaf("Stay", autoClose: false)));
        await OpenMenuAsync(cut);

        await cut.InvokeAsync(() => Child(cut, 0).Click());

        Assert.Single(cut.FindAll(RootPanel));
        Assert.Single(cut.FindAll(SubmenuPanel));
        Assert.Equal(1, _actions);
        Assert.Equal(0, _toggles);
    }

    [Fact]
    public async Task GroupedItem_StillClosesTheMenuOnce()
    {
        var cut = RenderMenu(b =>
        {
            b.OpenComponent<FlareMenuGroup>(0);
            b.AddAttribute(1, nameof(FlareMenuGroup.ChildContent), Leaf("Grouped"));
            b.CloseComponent();
        });
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Activator} button").Click(new MouseEventArgs { Detail = 1 }));

        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Group} [role=menuitem]").Click());

        Assert.Empty(cut.FindAll(RootPanel));
        Assert.Equal(1, _toggles);
    }
}
