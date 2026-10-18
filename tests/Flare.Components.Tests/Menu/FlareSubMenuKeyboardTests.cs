using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A submenu is part of the parent menu's keyboard model: its trigger is reachable with the arrows,
/// ArrowRight/Left and Enter open and leave it, tap opens it, Escape closes only the submenu, and focus
/// moves into the panel and back to the trigger.
/// </summary>
public class FlareSubMenuKeyboardTests : FlareTestContext
{
    private static MouseEventArgs FromKeyboard() => new() { Detail = 0 };
    private static KeyboardEventArgs Key(string key, bool shift = false) => new() { Key = key, ShiftKey = shift };

    private static RenderFragment<FlareMenuActivatorContext> ActivatorButton() => ctx => b =>
    {
        b.OpenElement(0, "button");
        b.AddMultipleAttributes(1, ctx.Attributes);
        b.AddContent(2, "open");
        b.CloseElement();
    };

    private RenderFragment MenuWithSubmenu(Action parent, Action first, Action second, bool disabled = false) => b =>
    {
        b.OpenComponent<FlareMenuItem>(0);
        b.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(cb => cb.AddContent(0, "Parent")));
        b.AddAttribute(2, nameof(FlareMenuItem.OnClick), EventCallback.Factory.Create(this, parent));
        b.CloseComponent();

        b.OpenComponent<FlareSubMenu>(3);
        b.AddAttribute(4, nameof(FlareSubMenu.Label), "More");
        b.AddAttribute(5, nameof(FlareSubMenu.Disabled), disabled);
        b.AddAttribute(6, nameof(FlareSubMenu.ChildContent), (RenderFragment)(sb =>
        {
            sb.OpenComponent<FlareMenuItem>(0);
            sb.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(cb => cb.AddContent(0, "First")));
            sb.AddAttribute(2, nameof(FlareMenuItem.OnClick), EventCallback.Factory.Create(this, first));
            sb.CloseComponent();
            sb.OpenComponent<FlareMenuItem>(3);
            sb.AddAttribute(4, nameof(FlareMenuItem.ChildContent), (RenderFragment)(cb => cb.AddContent(0, "Second")));
            sb.AddAttribute(5, nameof(FlareMenuItem.OnClick), EventCallback.Factory.Create(this, second));
            sb.CloseComponent();
        }));
        b.CloseComponent();
    };

    private IRenderedComponent<FlareMenu> OpenMenu(RenderFragment content)
        => Render<FlareMenu>(p => p
            .Add(m => m.Activator, ActivatorButton())
            .Add(m => m.ChildContent, content));

    private static string PanelSelector => $".{Css.Classes.Menu.Panel}";
    private static string SubmenuPanelSelector => $".{Css.Classes.Menu.SubmenuPanel}";
    private static string SubmenuActivatorSelector => $".{Css.Classes.Menu.SubmenuActivator}";

    [Fact]
    public async Task ArrowRight_OpensTheSubmenuAndFocusesIt()
    {
        var cut = OpenMenu(MenuWithSubmenu(() => { }, () => { }, () => { }));
        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(FromKeyboard()));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("ArrowDown")));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("ArrowRight")));

        Assert.Single(cut.FindAll(SubmenuPanelSelector));
        cut.WaitForAssertion(() => Assert.False(string.IsNullOrEmpty(
            cut.Find(SubmenuPanelSelector).GetAttribute("aria-activedescendant"))));
    }

    [Fact]
    public async Task Enter_OpensTheSubmenu()
    {
        var cut = OpenMenu(MenuWithSubmenu(() => { }, () => { }, () => { }));
        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(FromKeyboard()));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("ArrowDown")));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("Enter")));

        Assert.Single(cut.FindAll(SubmenuPanelSelector));
    }

    [Fact]
    public async Task Click_OpensTheSubmenu()
    {
        var cut = OpenMenu(MenuWithSubmenu(() => { }, () => { }, () => { }));
        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(FromKeyboard()));

        await cut.InvokeAsync(() => cut.Find(SubmenuActivatorSelector).Click());

        Assert.Single(cut.FindAll(SubmenuPanelSelector));
    }

    [Fact]
    public async Task Escape_InSubmenu_ClosesOnlyTheSubmenu()
    {
        var cut = OpenMenu(MenuWithSubmenu(() => { }, () => { }, () => { }));
        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(FromKeyboard()));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("ArrowDown")));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("ArrowRight")));

        await cut.InvokeAsync(() => cut.Find(SubmenuPanelSelector).KeyDown(Key("Escape")));

        Assert.Empty(cut.FindAll(SubmenuPanelSelector));
        Assert.Single(cut.FindAll(PanelSelector));       // the parent menu is still there
    }

    [Fact]
    public async Task ArrowLeft_ReturnsToTheParentMenu()
    {
        var cut = OpenMenu(MenuWithSubmenu(() => { }, () => { }, () => { }));
        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(FromKeyboard()));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("ArrowDown")));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("ArrowRight")));

        await cut.InvokeAsync(() => cut.Find(SubmenuPanelSelector).KeyDown(Key("ArrowLeft")));

        Assert.Empty(cut.FindAll(SubmenuPanelSelector));
        Assert.Single(cut.FindAll(PanelSelector));
    }

    [Fact]
    public async Task Enter_InsideSubmenu_ActivatesTheItem()
    {
        var first = 0;
        var cut = OpenMenu(MenuWithSubmenu(() => { }, () => first++, () => { }));
        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(FromKeyboard()));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("ArrowDown")));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("ArrowRight")));
        cut.WaitForAssertion(() => Assert.False(string.IsNullOrEmpty(
            cut.Find(SubmenuPanelSelector).GetAttribute("aria-activedescendant"))));
        await cut.InvokeAsync(() => cut.Find(SubmenuPanelSelector).KeyDown(Key("Enter")));

        Assert.Equal(1, first);
        Assert.Empty(cut.FindAll(PanelSelector));       // AutoClose on the child closed the whole chain
    }

    [Fact]
    public async Task DisabledSubmenu_IsSkippedAndDoesNotOpen()
    {
        var parent = 0;
        var cut = OpenMenu(MenuWithSubmenu(() => parent++, () => { }, () => { }, disabled: true));
        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(FromKeyboard()));

        await cut.InvokeAsync(() => cut.Find(SubmenuActivatorSelector).Click());
        Assert.Empty(cut.FindAll(SubmenuPanelSelector));

        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("ArrowDown")));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("Enter")));

        Assert.Equal(1, parent);                         // the only enabled entry was activated
        Assert.Empty(cut.FindAll(SubmenuPanelSelector));
    }

    [Fact]
    public async Task NestedSubmenu_OpensFromTheOuterSubmenu()
    {
        var cut = OpenMenu(b =>
        {
            b.OpenComponent<FlareSubMenu>(0);
            b.AddAttribute(1, nameof(FlareSubMenu.Label), "More");
            b.AddAttribute(2, nameof(FlareSubMenu.ChildContent), (RenderFragment)(sb =>
            {
                sb.OpenComponent<FlareSubMenu>(0);
                sb.AddAttribute(1, nameof(FlareSubMenu.Label), "Inner");
                sb.AddAttribute(2, nameof(FlareSubMenu.ChildContent), (RenderFragment)(ib =>
                {
                    ib.OpenComponent<FlareMenuItem>(0);
                    ib.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(cb => cb.AddContent(0, "Deep")));
                    ib.CloseComponent();
                }));
                sb.CloseComponent();
            }));
            b.CloseComponent();
        });

        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(FromKeyboard()));
        await cut.InvokeAsync(() => cut.Find(PanelSelector).KeyDown(Key("Enter")));   // opens outer
        await cut.InvokeAsync(() => cut.FindAll(SubmenuPanelSelector)[0].KeyDown(Key("ArrowRight"))); // opens inner

        Assert.Equal(2, cut.FindAll(SubmenuPanelSelector).Count);
    }
}
