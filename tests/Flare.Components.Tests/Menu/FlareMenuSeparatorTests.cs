using Flare.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

/// <summary>
/// A separator is a visual rule, not a command: it reports role=separator, takes no tab stop and is
/// invisible to the menu's roving keyboard navigation because it never registers as an entry.
/// </summary>
public class FlareMenuSeparatorTests : FlareTestContext
{
    private static RenderFragment<FlareMenuActivatorContext> ActivatorButton() => ctx => b =>
    {
        b.OpenElement(0, "button");
        b.AddMultipleAttributes(1, ctx.Attributes);
        b.AddContent(2, "open");
        b.CloseElement();
    };

    private static RenderFragment TwoItemsAroundSeparator() => b =>
    {
        b.OpenComponent<FlareMenuItem>(0);
        b.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(cb => cb.AddContent(0, "First")));
        b.CloseComponent();
        b.OpenComponent<FlareMenuSeparator>(2);
        b.CloseComponent();
        b.OpenComponent<FlareMenuItem>(3);
        b.AddAttribute(4, nameof(FlareMenuItem.ChildContent), (RenderFragment)(cb => cb.AddContent(0, "Second")));
        b.CloseComponent();
    };

    private static RenderFragment MenuWithSubmenuSeparator() => b =>
    {
        b.OpenComponent<FlareSubMenu>(0);
        b.AddAttribute(1, nameof(FlareSubMenu.Label), "More");
        b.AddAttribute(2, nameof(FlareSubMenu.HideDelay), 0);
        b.AddAttribute(3, nameof(FlareSubMenu.ChildContent), (RenderFragment)(sb =>
        {
            sb.OpenComponent<FlareMenuItem>(0);
            sb.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(cb => cb.AddContent(0, "First")));
            sb.CloseComponent();
            sb.OpenComponent<FlareMenuSeparator>(2);
            sb.CloseComponent();
            sb.OpenComponent<FlareMenuItem>(3);
            sb.AddAttribute(4, nameof(FlareMenuItem.ChildContent), (RenderFragment)(cb => cb.AddContent(0, "Second")));
            sb.CloseComponent();
        }));
        b.CloseComponent();
    };

    private IRenderedComponent<FlareMenu> OpenMenu(RenderFragment content) =>
        Render<FlareMenu>(p => p
            .Add(m => m.Activator, ActivatorButton())
            .Add(m => m.ChildContent, content));

    [Fact]
    public async Task RendersRoleSeparatorWithoutATabStop()
    {
        var cut = OpenMenu(TwoItemsAroundSeparator());
        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(new MouseEventArgs { Detail = 0 }));

        var separator = cut.Find($".{Css.Classes.Menu.Separator}");
        Assert.Equal("div", separator.TagName.ToLowerInvariant());
        Assert.Equal("separator", separator.GetAttribute("role"));
        Assert.Equal("horizontal", separator.GetAttribute("aria-orientation"));
        Assert.Null(separator.GetAttribute("tabindex"));
    }

    [Fact]
    public async Task ArrowNavigationMovesBetweenItemsAndNeverLandsOnTheSeparator()
    {
        var cut = OpenMenu(TwoItemsAroundSeparator());
        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(new MouseEventArgs { Detail = 0 }));

        var items = cut.FindAll($".{Css.Classes.Menu.Item}");
        Assert.Equal(2, items.Count);
        Assert.Equal(items[0].Id, cut.Find("[role=menu]").GetAttribute("aria-activedescendant"));

        await cut.InvokeAsync(() => cut.Find("[role=menu]").KeyDown(new KeyboardEventArgs { Key = "ArrowDown" }));

        // The next stop is the second registered item, not the separator between them.
        Assert.Equal(items[1].Id, cut.Find("[role=menu]").GetAttribute("aria-activedescendant"));
    }

    [Fact]
    public async Task RendersInsideAnOpenSubmenu()
    {
        var cut = OpenMenu(MenuWithSubmenuSeparator());
        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(new MouseEventArgs { Detail = 0 }));
        await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.SubmenuActivator}").Click());

        var separator = cut.Find($".{Css.Classes.Menu.SubmenuPanel} [role=separator]");
        Assert.Equal("separator", separator.GetAttribute("role"));
        Assert.Null(separator.GetAttribute("tabindex"));
    }
}
