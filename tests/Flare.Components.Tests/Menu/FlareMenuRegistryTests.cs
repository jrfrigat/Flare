using AngleSharp.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;

namespace Flare.Components.Tests;

public class FlareMenuRegistryTests : FlareTestContext
{
    private static string Panel => $".{Css.Classes.Menu.Panel}";
    private static string Focused => $".{Css.Classes.Menu.ItemFocused}";

    private static async Task OpenMenuAsync(IRenderedComponent<DynamicMenuHost> cut)
        => await cut.InvokeAsync(() => cut.Find($".{Css.Classes.Menu.Activator} button").Click(new MouseEventArgs { Detail = 0 }));

    private static Task PressAsync(IRenderedComponent<DynamicMenuHost> cut, string key)
        => cut.InvokeAsync(() => cut.Find(Panel).KeyDown(new KeyboardEventArgs { Key = key }));

    private static string FocusedLabel(IRenderedComponent<DynamicMenuHost> cut)
        => cut.Find(Focused).TextContent.Trim();

    private static void SetItems(IRenderedComponent<DynamicMenuHost> cut, params MenuEntry[] items)
        => cut.Render(p => p.Add(x => x.Items, items));

    [Fact]
    public async Task RemovingTheFocusedItem_MovesTheHighlightToTheNextSurvivor()
    {
        var cut = Render<DynamicMenuHost>(p => p.Add(x => x.Items,
            new MenuEntry[] { new("a", "A"), new("b", "B"), new("c", "C") }));
        await OpenMenuAsync(cut);
        await PressAsync(cut, "ArrowDown");
        Assert.Equal("B", FocusedLabel(cut));
        var removedId = cut.Find(Panel).GetAttribute("aria-activedescendant");

        SetItems(cut, new MenuEntry("a", "A"), new MenuEntry("c", "C"));

        cut.WaitForAssertion(() => Assert.Equal("C", FocusedLabel(cut)));
        Assert.DoesNotContain(removedId, cut.Markup);
        Assert.Equal(cut.Find(Focused).Id, cut.Find(Panel).GetAttribute("aria-activedescendant"));

        await PressAsync(cut, "ArrowDown");
        Assert.Equal("A", FocusedLabel(cut));
    }

    [Fact]
    public async Task RemovingAnotherItem_KeepsTheHighlightOnItsItem()
    {
        var cut = Render<DynamicMenuHost>(p => p.Add(x => x.Items,
            new MenuEntry[] { new("a", "A"), new("b", "B"), new("c", "C") }));
        await OpenMenuAsync(cut);
        await PressAsync(cut, "ArrowDown");

        SetItems(cut, new MenuEntry("b", "B"), new MenuEntry("c", "C"));

        cut.WaitForAssertion(() => Assert.Equal("B", FocusedLabel(cut)));
        await PressAsync(cut, "ArrowDown");
        Assert.Equal("C", FocusedLabel(cut));
    }

    [Fact]
    public async Task Reordering_KeyboardFollowsTheRenderedOrder()
    {
        var cut = Render<DynamicMenuHost>(p => p
            .Add(x => x.Keyed, false)
            .Add(x => x.Items, new MenuEntry[] { new("a", "A"), new("b", "B"), new("c", "C") }));
        await OpenMenuAsync(cut);
        await PressAsync(cut, "ArrowDown");
        Assert.Equal("B", FocusedLabel(cut));

        SetItems(cut, new MenuEntry("c", "C"), new MenuEntry("a", "A"), new MenuEntry("b", "B"));
        Assert.Equal("A", FocusedLabel(cut));

        await PressAsync(cut, "ArrowDown");
        Assert.Equal("B", FocusedLabel(cut));
        await PressAsync(cut, "ArrowUp");
        Assert.Equal("A", FocusedLabel(cut));
    }

    [Fact]
    public async Task RemovingAnOpenSubmenu_DropsItFromTheParentRegistry()
    {
        var cut = Render<DynamicMenuHost>(p => p
            .Add(x => x.ShowSubmenu, true)
            .Add(x => x.Items, new MenuEntry[] { new("a", "A"), new("b", "B") }));
        await OpenMenuAsync(cut);
        Assert.Equal("More", FocusedLabel(cut));

        cut.Render(p => p.Add(x => x.ShowSubmenu, false));

        cut.WaitForAssertion(() => Assert.Equal("A", FocusedLabel(cut)));
    }

    [Fact]
    public async Task DisabledItems_AreSkippedAfterARemoval()
    {
        var cut = Render<DynamicMenuHost>(p => p.Add(x => x.Items,
            new MenuEntry[] { new("a", "A"), new("b", "B"), new("c", "C", Disabled: true) }));
        await OpenMenuAsync(cut);
        await PressAsync(cut, "ArrowDown");
        Assert.Equal("B", FocusedLabel(cut));

        SetItems(cut, new MenuEntry("b", "B"), new MenuEntry("c", "C", Disabled: true),
            new MenuEntry("d", "D"));

        cut.WaitForAssertion(() => Assert.Equal("B", FocusedLabel(cut)));
        await PressAsync(cut, "ArrowDown");
        Assert.Equal("D", FocusedLabel(cut));
        await PressAsync(cut, "ArrowDown");
        Assert.Equal("B", FocusedLabel(cut));
    }

    public sealed record MenuEntry(string Key, string Label, bool Disabled = false);

    public sealed class DynamicMenuHost : ComponentBase
    {
        [Parameter] public IReadOnlyList<MenuEntry> Items { get; set; } = [];
        [Parameter] public bool Keyed { get; set; } = true;
        [Parameter] public bool ShowSubmenu { get; set; }

        protected override void BuildRenderTree(RenderTreeBuilder b)
        {
            b.OpenComponent<FlareMenu>(0);
            b.AddAttribute(1, nameof(FlareMenu.Activator), ActivatorButton());
            b.AddAttribute(2, nameof(FlareMenu.ChildContent), (RenderFragment)(content =>
            {
                var seq = 0;
                if (ShowSubmenu)
                {
                    content.OpenComponent<FlareSubMenu>(seq++);
                    content.SetKey("submenu");
                    content.AddAttribute(seq++, nameof(FlareSubMenu.Label), "More");
                    content.AddAttribute(seq++, nameof(FlareSubMenu.ChildContent),
                        (RenderFragment)(c =>
                        {
                            c.OpenComponent<FlareMenuItem>(0);
                            c.AddAttribute(1, nameof(FlareMenuItem.ChildContent), (RenderFragment)(l => l.AddContent(0, "Inner")));
                            c.CloseComponent();
                        }));
                    content.CloseComponent();
                }
                foreach (var item in Items)
                {
                    content.OpenComponent<FlareMenuItem>(seq++);
                    if (Keyed) content.SetKey(item.Key);
                    content.AddAttribute(seq++, nameof(FlareMenuItem.ChildContent),
                        (RenderFragment)(c => c.AddContent(0, item.Label)));
                    content.AddAttribute(seq++, nameof(FlareMenuItem.Disabled), item.Disabled);
                    content.CloseComponent();
                }
            }));
            b.CloseComponent();
        }

        private static RenderFragment<FlareMenuActivatorContext> ActivatorButton() => ctx => b =>
        {
            b.OpenElement(0, "button");
            b.AddMultipleAttributes(1, ctx.Attributes);
            b.AddContent(2, "open");
            b.CloseElement();
        };
    }
}
