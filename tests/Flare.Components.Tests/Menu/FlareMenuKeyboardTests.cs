using System.Globalization;
using Flare.Abstractions;
using Flare.Components;
using Flare.Abstractions.Tokens;
using Microsoft.AspNetCore.Components;

namespace Flare.Components.Tests;

// FlareMenu keyboard a11y: opening sets aria-activedescendant to the first item and arrow keys move it.
public class FlareMenuKeyboardTests : FlareTestContext
{
    private static RenderFragment ThreeItems() => b =>
    {
        for (var i = 0; i < 3; i++)
        {
            b.OpenComponent<FlareMenuItem>(i * 2);
            b.AddAttribute(i * 2 + 1, nameof(FlareMenuItem.ChildContent),
                (RenderFragment)(cb => cb.AddContent(0, $"Item {i}")));
            b.CloseComponent();
        }
    };

    // Enter/Space on the activator raises a click carrying no click count; a real press reports at least
    // one. That is what tells a keyboard opening from a pointer one, so both tests state it outright
    // rather than relying on what a bare Click() happens to send.
    private static Microsoft.AspNetCore.Components.Web.MouseEventArgs FromKeyboard() => new() { Detail = 0 };
    private static Microsoft.AspNetCore.Components.Web.MouseEventArgs FromPointer() => new() { Detail = 1 };

    [Fact]
    public async Task OpenedByKeyboard_SetsActiveDescendant_AndArrowMovesIt()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(m => m.Activator, "<span>open</span>")
            .Add(m => m.ChildContent, ThreeItems()));

        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(FromKeyboard()));

        var ad1 = cut.Find("[role=menu]").GetAttribute("aria-activedescendant");
        Assert.False(string.IsNullOrEmpty(ad1));                                  // points at the first item

        await cut.InvokeAsync(() => cut.Find("[role=menu]").KeyDown(
            new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" }));
        var ad2 = cut.Find("[role=menu]").GetAttribute("aria-activedescendant");
        Assert.NotEqual(ad1, ad2);                                               // moved to the next item
    }

    // Opening with the pointer must NOT highlight anything: the user is looking at where they clicked, and
    // a ring on the first item reads as "this is selected". The first arrow key still has to land on the
    // first item rather than skipping it, which is what the second half checks.
    [Fact]
    public async Task OpenedByPointer_HighlightsNothing_UntilTheFirstArrow()
    {
        var cut = Render<FlareMenu>(p => p
            .Add(m => m.Activator, "<span>open</span>")
            .Add(m => m.ChildContent, ThreeItems()));

        await cut.InvokeAsync(() => cut.Find("[aria-haspopup=menu]").Click(FromPointer()));

        Assert.True(string.IsNullOrEmpty(cut.Find("[role=menu]").GetAttribute("aria-activedescendant")));
        Assert.Empty(cut.FindAll("." + Flare.Css.Classes.Menu.ItemFocused));

        await cut.InvokeAsync(() => cut.Find("[role=menu]").KeyDown(
            new Microsoft.AspNetCore.Components.Web.KeyboardEventArgs { Key = "ArrowDown" }));

        var focused = cut.FindAll("." + Flare.Css.Classes.Menu.ItemFocused);
        Assert.Single(focused);
        var items = cut.FindAll("." + Flare.Css.Classes.Menu.Item);
        Assert.Equal(3, items.Count);                                             // not a vacuous comparison
        Assert.Equal(items[0].TextContent.Trim(), focused[0].TextContent.Trim());  // the first, not the second
    }
}
